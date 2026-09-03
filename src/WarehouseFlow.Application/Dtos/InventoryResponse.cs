using WarehouseFlow.Domain.Entities;

namespace WarehouseFlow.Application.Dtos;

public class InventoryResponse
{
    public string WarehouseName { get; set; } = string.Empty;

    public Guid Id { get; set; }
    public Product Product { get; set; } = null!;
    public int AvailableQuantity { get; set; }
    public int ReservedQuantity { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public DateTime? LastReservedAt { get; set; }
    public Guid WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;
}

public static class InventoryResponseFactory
{
    public static InventoryResponse FromInventory(Inventory inventory) =>
        new()
        {
            Id = inventory.Id,
            ProductName = inventory.Product.ProductName,
            AvailableQuantity = inventory.AvailableQuantity,
            ReservedQuantity = inventory.ReservedQuantity,
            WarehouseName = inventory.Warehouse.WarehouseName,
        };
}
