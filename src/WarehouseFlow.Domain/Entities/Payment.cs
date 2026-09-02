namespace WarehouseFlow.Domain.Entities;

public class Payment:BaseEntity
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public decimal Amount { get; set; }
}