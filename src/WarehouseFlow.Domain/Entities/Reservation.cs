namespace WarehouseFlow.Domain.Entities;

public class Reservation : BaseEntity
{
    public Guid OrderId { get; set; }
     public Order Order { get; set; } = null!;
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public Guid WarehouseId { get; set; }
     public Warehouse Warehouse { get; set; } = null!;
    public int ReservedQuantity { get; set; }
    public DateTime ExpiresAt { get; set; }
}
