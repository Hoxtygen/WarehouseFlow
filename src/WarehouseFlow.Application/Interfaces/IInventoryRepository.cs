using WarehouseFlow.Domain.Entities;

namespace WarehouseFlow.Application.Interfaces;

public interface IInventoryRepository
{
    Task<Inventory?> GetByIdAsync(Guid inventoryId, CancellationToken cancellationToken = default);
    Task<Inventory?> GetByProductAndWarehouseAsync(
        Guid productId,
        Guid warehouseId,
        CancellationToken cancellationToken = default
    );
    Task<IList<Inventory>> GetLockedByProductAsync(
        Guid productId,
        CancellationToken cancellationToken = default
    );
    Task<bool> ExistsAsync(Guid productId, Guid warehouseId, CancellationToken cancellationToken = default);
    Task<int> SumOccupiedCapacityAsync(
        Guid warehouseId,
        CancellationToken cancellationToken = default
    );
    Task ReleaseReservedStockAsync(
        Guid warehouseId,
        Guid productId,
        int quantity,
        DateTime now,
        CancellationToken cancellationToken = default
    );
    Task AddAsync(Inventory inventory, CancellationToken cancellationToken = default);
}
