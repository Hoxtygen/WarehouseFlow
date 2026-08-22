

using WarehouseFlow.Domain.Entities;

public class Inventory:BaseEntity
{
     public Guid ProductId { get; set; }
    public int AvailableQuantity { get; set; }
    public int ReservedQuantity { get; set; }
    public Product Product { get; set; } = null!;
    public DateTime? LastReservedAt { get; set; }
    public Guid WarehouseId {get; set;}
    public Warehouse Warehouse {get; set;} = null!;
}
