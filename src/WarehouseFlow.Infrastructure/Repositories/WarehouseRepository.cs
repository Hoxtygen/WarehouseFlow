using Microsoft.EntityFrameworkCore;
using WarehouseFlow.Application.Interfaces;
using WarehouseFlow.Domain.Entities;
using WarehouseFlow.Infrastructure.Data;

namespace WarehouseFlow.Infrastructure.Repositories;

public class WarehouseRepository(AppDbContext dbContext) : IWarehouseRepository
{
    public async Task<bool> ExistsAsync(Guid warehouseId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Warehouses.AnyAsync(
            warehouse => warehouse.Id == warehouseId,
            cancellationToken
        );
    }

    public async Task<int> GetCapacityAsync(Guid warehouseId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Warehouses
            .Where(warehouse => warehouse.Id == warehouseId)
            .Select(warehouse => warehouse.Capacity)
            .SingleAsync(cancellationToken);
    }

    public async Task AddAsync(Warehouse warehouse, CancellationToken cancellationToken = default)
    {
        await dbContext.AddAsync(warehouse, cancellationToken);
    }
}
