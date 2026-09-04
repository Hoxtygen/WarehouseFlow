using WarehouseFlow.Domain.Entities;

namespace WarehouseFlow.Application.Interfaces;

public interface IOrderRepository
{
    Task AddAsync(Order order, CancellationToken cancellationToken = default);
    Task<Order?> GetByIdWithDetailsAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<IList<Reservation>> GetExpiredReservationsAsync(
        DateTime now,
        CancellationToken cancellationToken = default
    );
    Task<bool> MarkAsCancelledAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task<bool> MarkAsPaidAsync(Guid orderId, CancellationToken cancellationToken = default);
    Task DeleteReservationsAsync(
        IEnumerable<Reservation> reservations,
        CancellationToken cancellationToken = default
    );
}
