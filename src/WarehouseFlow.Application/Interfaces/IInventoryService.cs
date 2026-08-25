using WarehouseFlow.Application.Dtos.WarehouseFlow.Application.Dtos;

namespace WarehouseFlow.Application.Interfaces
{
    public interface IInventoryService
    {
        Task<Inventory> CreateInventory(
            InventoryDto inventoryDto,
            CancellationToken cancellationToken = default
        );
    }
}
