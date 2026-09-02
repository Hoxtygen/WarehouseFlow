using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WarehouseFlow.Application.Dtos;
using WarehouseFlow.Application.Interfaces;
using WarehouseFlow.Domain.Exceptions;
using WarehouseFlow.Infrastructure.Data;

namespace WarehouseFlow.Infrastructure.Implementations
{
    public sealed class InventoryService(
        AppDbContext dbContext,
        IWarehouseService warehouseService,
        IProductService productService,
        ILogger<InventoryService> logger
    ) : IInventoryService
    {
        public async Task<Inventory> CreateInventory(
            InventoryDto inventoryDto,
            CancellationToken cancellationToken = default
        )
        {
            if (inventoryDto.AvailableQuantity <= 0 || inventoryDto.ReservedQuantity <= 0)
            {
                throw new ValidationException(
                    "Inventory quantities cannot be negative.",
                    ["Available and reserved quantities must be greater than 0."]
                );
            }

            if (inventoryDto.ReservedQuantity > inventoryDto.AvailableQuantity)
            {
                throw new ValidationException(
                    "Invalid inventory quantities.",
                    ["Available quantity must be greater than or equal to reserved quantity."]
                );
            }

            if (!await productService.ProductExists(inventoryDto.ProductId, cancellationToken))
            {
                throw new InvalidDataException("Product does not exist");
            }

            if (
                !await warehouseService.WarehouseExists(inventoryDto.WarehouseId, cancellationToken)
            )
            {
                throw new InvalidDataException("Warehouse does not exist");
            }

            if (
                await InventoryExists(
                    inventoryDto.ProductId,
                    inventoryDto.WarehouseId,
                    cancellationToken
                )
            )
            {
                throw new DuplicateException(
                    "This product is already inventoried in the selected warehouse."
                );
            }

            var warehouseCapacity = await dbContext
                .Warehouses.Where(warehouse => warehouse.Id == inventoryDto.WarehouseId)
                .Select(warehouse => warehouse.Capacity)
                .SingleAsync(cancellationToken);

            var occupiedCapacity =
                await dbContext
                    .Inventories.Where(inventory =>
                        inventory.WarehouseId == inventoryDto.WarehouseId
                    )
                    .SumAsync(
                        inventory =>
                            (int?)(inventory.AvailableQuantity + inventory.ReservedQuantity),
                        cancellationToken
                    )
                ?? 0;

            var requestedCapacity = inventoryDto.AvailableQuantity + inventoryDto.ReservedQuantity;

            if (occupiedCapacity + requestedCapacity > warehouseCapacity)
            {
                throw new ValidationException(
                    "Warehouse capacity exceeded.",
                    [
                        $"The warehouse has {warehouseCapacity - occupiedCapacity} remaining capacity.",
                    ]
                );
            }

            Inventory inventory = new Inventory()
            {
                AvailableQuantity = inventoryDto.AvailableQuantity,
                ProductId = inventoryDto.ProductId,
                ReservedQuantity = inventoryDto.ReservedQuantity,
                WarehouseId = inventoryDto.WarehouseId,
            };

            dbContext.Inventories.Add(inventory);
            await dbContext.SaveChangesAsync(cancellationToken);
            logger.LogInformation(
                "Inventory created successfully for Product {ProductId} in Warehouse {WarehouseId}",
                inventory.ProductId,
                inventory.WarehouseId
            );

            return inventory;
        }

        public async Task<Inventory> GetInventoryByIdAsync(
            Guid inventoryId,
            CancellationToken cancellationToken = default
        )
        {
            var inventory = await dbContext
                .Inventories.AsNoTracking()
                .FirstOrDefaultAsync(inventory => inventory.Id == inventoryId, cancellationToken);
            if (inventory is null)
            {
                throw new NotFoundException("Inventory not found");
            }
            return inventory;
        }

        public async Task<Inventory> GetInventoryByProductAndWarehouseAsync(
            Guid productId,
            Guid warehouseId,
            CancellationToken cancellationToken = default
        )
        {
            var inventory = await dbContext
                .Inventories.FirstOrDefaultAsync(
                    inventory =>
                        inventory.ProductId == productId && inventory.WarehouseId == warehouseId,
                    cancellationToken
                );

            if (inventory is null)
            {
                logger.LogError(
                    "Inventory not found for Product {ProductId} in Warehouse {WarehouseId}",
                    productId,
                    warehouseId
                );
                throw new NotFoundException(
                    $"Inventory not found for Product {productId} in Warehouse {warehouseId}"
                );
            }

            return inventory;
        }

        public async Task<IList<InventoryReservationDto>> ReserveInventoryAsync(
            Guid productId,
            int quantity,
            CancellationToken cancellationToken = default
        )
        {
            var inventories = await dbContext
                .Inventories.FromSqlInterpolated(
                    $"SELECT * FROM inventories WHERE \"ProductId\" = {productId} ORDER BY \"Id\" FOR UPDATE"
                )
                .ToListAsync(cancellationToken);

            var totalAvailable = inventories.Sum(inventory => inventory.AvailableQuantity);

            if (totalAvailable < quantity)
            {
                logger.LogError(
                    "Insufficient stock for Product {ProductId}. Available: {Available}, Requested: {Requested}",
                    productId,
                    totalAvailable,
                    quantity
                );
                throw new InsufficientStockException(
                    $"Insufficient stock for Product {productId}. Available: {totalAvailable}, Requested: {quantity}"
                );
            }

            var reservations = new List<InventoryReservationDto>();
            var remainingProductQuantityToReserve = quantity;

            foreach (
                var inventory in inventories.Where(inventory => inventory.AvailableQuantity > 0)
            )
            {
                if (remainingProductQuantityToReserve <= 0)
                {
                    break;
                }

                var productQuantityToReserve = Math.Min(
                    inventory.AvailableQuantity,
                    remainingProductQuantityToReserve
                );
                inventory.AvailableQuantity -= productQuantityToReserve;
                inventory.ReservedQuantity += productQuantityToReserve;
                inventory.LastReservedAt = DateTime.UtcNow;
                remainingProductQuantityToReserve -= productQuantityToReserve;

                reservations.Add(
                    new InventoryReservationDto(inventory.WarehouseId, productQuantityToReserve)
                );
                logger.LogInformation("Reservation has been made : {reservations}", reservations);
            }

            //registers the inventory changes to the current transaction context
            await dbContext.SaveChangesAsync(cancellationToken);
            logger.LogInformation(
                "Reserved {Quantity} units of Product {ProductId} from {WarehouseCount} warehouse(s)",
                quantity,
                productId,
                reservations.Count
            );

            return reservations;
        }

        private async Task<bool> InventoryExists(
            Guid productId,
            Guid warehouseId,
            CancellationToken cancellationToken
        )
        {
            return await dbContext.Inventories.AnyAsync(
                inventory =>
                    inventory.ProductId == productId && inventory.WarehouseId == warehouseId,
                cancellationToken
            );
        }
    }
}
