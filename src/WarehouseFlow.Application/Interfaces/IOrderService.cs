using WarehouseFlow.Application.Dtos;
using WarehouseFlow.Domain.Entities;

namespace WarehouseFlow.Application.Interfaces
{
    public interface IOrderService
    {
        Task<Order> CreateOrder(
            OrderDto orderDto,
            string applicationUserId,
            CancellationToken cancellationToken
        );

        Task ExpireReservationsAsync(CancellationToken cancellationToken = default);
    }
}
