using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WarehouseFlow.Application.Dtos.WarehouseFlow.Application.Dtos;
using WarehouseFlow.Application.Interfaces;
using WarehouseFlow.Domain.Exceptions;
using WarehouseFlow.Infrastructure.Data;

namespace WarehouseFlow.Infrastructure.Implementations
{
    public sealed class InventoryService(AppDbContext dbContext, ILogger<InventoryService> logger)
        : IInventoryService
    {
        public async Task<Inventory> CreateInventory(
            InventoryDto inventoryDto,
            CancellationToken cancellationToken = default
        )
        {
            if (
                inventoryDto.AvailableQuantity < 0
                || inventoryDto.ReservedQuantity < 0
            )
            {
                throw new ValidationException(
                    "Inventory quantities cannot be negative.",
                    ["Available and reserved quantities must be zero or greater."]
                );
            }

            if (inventoryDto.ReservedQuantity > inventoryDto.AvailableQuantity)
            {
                throw new ValidationException(
                    "Invalid inventory quantities.",
                    ["Available quantity must be greater than or equal to reserved quantity."]
                );
            }

            if (!await ProductExists(inventoryDto.ProductId, cancellationToken))
            {
                throw new InvalidDataException("Product does not exist");
            }

            if (!await WarehouseExists(inventoryDto.WarehouseId, cancellationToken))
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

            var warehouseCapacity = await dbContext.Warehouses
                .Where(warehouse => warehouse.Id == inventoryDto.WarehouseId)
                .Select(warehouse => warehouse.Capacity)
                .SingleAsync(cancellationToken);

            var occupiedCapacity = await dbContext.Inventories
                .Where(inventory => inventory.WarehouseId == inventoryDto.WarehouseId)
                .SumAsync(
                    inventory => inventory.AvailableQuantity + inventory.ReservedQuantity,
                    cancellationToken
                );

            var requestedCapacity =
                inventoryDto.AvailableQuantity + inventoryDto.ReservedQuantity;

            if (occupiedCapacity + requestedCapacity > warehouseCapacity)
            {
                throw new ValidationException(
                    "Warehouse capacity exceeded.",
                    [
                        $"The warehouse has {warehouseCapacity - occupiedCapacity} remaining capacity."
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

        private async Task<bool> ProductExists(Guid productId, CancellationToken cancellationToken)
        {
            return await dbContext.Products.AnyAsync(p => p.Id == productId, cancellationToken);
        }

        private async Task<bool> WarehouseExists(
            Guid warehouseId,
            CancellationToken cancellationToken
        )
        {
            return await dbContext.Warehouses.AnyAsync(w => w.Id == warehouseId, cancellationToken);
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
