using Microsoft.EntityFrameworkCore;
using WarehouseFlow.Application.Interfaces;
using WarehouseFlow.Domain.Entities;
using WarehouseFlow.Domain.Enum;
using WarehouseFlow.Infrastructure.Data;

namespace WarehouseFlow.Infrastructure.Repositories;

public class OrderRepository(AppDbContext dbContext) : IOrderRepository
{
    public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        await dbContext.Orders.AddAsync(order, cancellationToken);
    }

    public async Task<Order?> GetByIdWithDetailsAsync(
        Guid orderId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext
            .Orders.AsNoTracking()
            .Include(order => order.OrderItems)
            .Include(order => order.Reservations)
            .FirstOrDefaultAsync(order => order.Id == orderId, cancellationToken);
    }

    public async Task<IList<Reservation>> GetExpiredReservationsAsync(
        DateTime now,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext
            .Reservations.Include(reservation => reservation.Order)
            .Where(reservation =>
                reservation.ExpiresAt < now
                && reservation.Order.OrderStatus != OrderStatus.Paid
                && reservation.Order.OrderStatus != OrderStatus.Cancelled
            )
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> MarkAsCancelledAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var affectedRows = await dbContext
            .Orders.Where(order => order.Id == orderId)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(order => order.OrderStatus, OrderStatus.Cancelled),
                cancellationToken
            );
        return affectedRows > 0;
    }

    public async Task<bool> MarkAsPaidAsync(Guid orderId, CancellationToken cancellationToken = default)
    {
        var affectedRows = await dbContext
            .Orders.Where(order => order.Id == orderId)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(order => order.OrderStatus, OrderStatus.Paid),
                cancellationToken
            );
        return affectedRows > 0;
    }

    public async Task DeleteReservationsAsync(
        IEnumerable<Reservation> reservations,
        CancellationToken cancellationToken = default
    )
    {
        dbContext.Reservations.RemoveRange(reservations);
        await Task.CompletedTask;
    }
}
