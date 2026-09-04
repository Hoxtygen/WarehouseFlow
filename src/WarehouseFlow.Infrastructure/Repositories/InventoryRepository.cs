using Microsoft.EntityFrameworkCore;
using WarehouseFlow.Application.Interfaces;
using WarehouseFlow.Domain.Entities;
using WarehouseFlow.Infrastructure.Data;

namespace WarehouseFlow.Infrastructure.Repositories;

public class InventoryRepository(AppDbContext dbContext) : IInventoryRepository
{
    public async Task<Inventory?> GetByIdAsync(
        Guid inventoryId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext.Inventories.AsNoTracking().FirstOrDefaultAsync(
            inventory => inventory.Id == inventoryId,
            cancellationToken
        );
    }

    public async Task<Inventory?> GetByProductAndWarehouseAsync(
        Guid productId,
        Guid warehouseId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext.Inventories.FirstOrDefaultAsync(
            inventory =>
                inventory.ProductId == productId && inventory.WarehouseId == warehouseId,
            cancellationToken
        );
    }

    public async Task<IList<Inventory>> GetLockedByProductAsync(
        Guid productId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext
            .Inventories.FromSqlInterpolated(
                $"SELECT * FROM inventories WHERE \"ProductId\" = {productId} ORDER BY \"Id\" FOR UPDATE"
            )
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> ExistsAsync(
        Guid productId,
        Guid warehouseId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext.Inventories.AnyAsync(
            inventory =>
                inventory.ProductId == productId && inventory.WarehouseId == warehouseId,
            cancellationToken
        );
    }

    public async Task<int> SumOccupiedCapacityAsync(
        Guid warehouseId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext.Inventories
                .Where(inventory => inventory.WarehouseId == warehouseId)
                .SumAsync(
                    inventory => (int?)(inventory.AvailableQuantity + inventory.ReservedQuantity),
                    cancellationToken
                )
            ?? 0;
    }

    public async Task ReleaseReservedStockAsync(
        Guid warehouseId,
        Guid productId,
        int quantity,
        DateTime now,
        CancellationToken cancellationToken = default
    )
    {
        await dbContext
            .Inventories.Where(inventory =>
                inventory.WarehouseId == warehouseId && inventory.ProductId == productId
            )
            .ExecuteUpdateAsync(
                setters =>
                    setters
                        .SetProperty(
                            inventory => inventory.AvailableQuantity,
                            inventory => inventory.AvailableQuantity + quantity
                        )
                        .SetProperty(
                            inventory => inventory.ReservedQuantity,
                            inventory => inventory.ReservedQuantity - quantity
                        )
                        .SetProperty(inventory => inventory.LastReservedAt, now),
                cancellationToken
            );
    }

    public async Task AddAsync(Inventory inventory, CancellationToken cancellationToken = default)
    {
        await dbContext.Inventories.AddAsync(inventory, cancellationToken);
    }

  
}
