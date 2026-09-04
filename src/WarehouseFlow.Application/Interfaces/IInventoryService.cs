using WarehouseFlow.Application.Dtos;

namespace WarehouseFlow.Application.Interfaces
{
    public interface IInventoryService
    {
        Task<InventoryResponse> CreateInventory(
            InventoryDto inventoryDto,
            CancellationToken cancellationToken = default
        );

        Task<IList<InventoryReservationDto>> ReserveInventoryAsync(
            Guid productId,
            int quantity,
            CancellationToken cancellationToken = default
        );

        Task<Inventory> GetInventoryByIdAsync(
            Guid inventoryId,
            CancellationToken cancellationToken = default
        );

        Task<Inventory> GetInventoryByProductAndWarehouseAsync(
            Guid productId,
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
    }
}
