using WarehouseFlow.Domain.Entities;

namespace WarehouseFlow.Application.Interfaces;

public interface IWarehouseRepository
{
    Task<bool> ExistsAsync(Guid warehouseId, CancellationToken cancellationToken = default);
    Task<int> GetCapacityAsync(Guid warehouseId, CancellationToken cancellationToken = default);
    Task AddAsync(Warehouse warehouse, CancellationToken cancellationToken = default);
}
