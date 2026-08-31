using WarehouseFlow.Application.Dtos;

namespace WarehouseFlow.Application.Interfaces
{
    public interface IInventoryService
    {
        Task<Inventory> CreateInventory(
            InventoryDto inventoryDto,
            CancellationToken cancellationToken = default
        );

        Task<IList<InventoryReservationDto>> ReserveInventoryAsync(
            Guid productId,
            int quantity,
            CancellationToken cancellationToken = default
        );
    }
}
