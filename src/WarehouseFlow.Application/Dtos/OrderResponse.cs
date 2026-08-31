using WarehouseFlow.Domain.Entities;
using WarehouseFlow.Domain.Enum;

namespace WarehouseFlow.Application.Dtos;

public class OrderResponse
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus OrderStatus { get; set; }
}

public static class OrderResponseFactory
{
    public static OrderResponse FromOrder(Order order) => new()
    {
        Id = order.Id,
        CustomerId = order.CustomerId,
        TotalAmount = order.TotalAmount,
        OrderDate = order.OrderDate,
        OrderStatus = order.OrderStatus
    };
}
