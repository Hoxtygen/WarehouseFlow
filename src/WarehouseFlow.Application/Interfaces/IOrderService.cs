using WarehouseFlow.Application.Dtos;
using WarehouseFlow.Domain.Entities;

namespace WarehouseFlow.Application.Interfaces
{
    public interface IOrderService
    {
        Task<OrderResponse> CreateOrder(
            OrderDto orderDto,
            string applicationUserId,
            CancellationToken cancellationToken
        );

        Task ExpireReservationsAsync(CancellationToken cancellationToken = default);
        Task<Order> GetOrderByIdAsync(Guid orderId, CancellationToken cancellationToken = default);
        Task MarkOrderAsPaidAsync(Guid orderId, CancellationToken cancellationToken = default);
    }
}
