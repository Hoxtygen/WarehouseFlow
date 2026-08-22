using WarehouseFlow.Domain.Enum;

namespace WarehouseFlow.Domain.Entities;

public class Order : BaseEntity
{
    public Guid CustomerId { get; set; }
    public Customer Customer {get;set;} = null!;
    public decimal TotalAmount { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus OrderStatus { get; set; }

     public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
