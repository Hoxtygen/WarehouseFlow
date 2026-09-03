using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WarehouseFlow.Application.Dtos;
using WarehouseFlow.Application.Interfaces;
using WarehouseFlow.Domain.Entities;
using WarehouseFlow.Domain.Enum;
using WarehouseFlow.Domain.Exceptions;
using WarehouseFlow.Infrastructure.Data;

namespace WarehouseFlow.Infrastructure.Implementations
{
    public sealed class OrderService(
        AppDbContext dbContext,
        IInventoryService inventoryService,
        IProductService productService,
        ICustomerService customerService,
        ILogger<OrderService> logger
    ) : IOrderService
    {
        public async Task<OrderResponse> CreateOrder(
            OrderDto orderDto,
            string applicationUserId,
            CancellationToken cancellationToken
        )
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(
                cancellationToken
            );

            try
            {
                var customer = await customerService.GetCustomerByApplicationUserIdAsync(
                    applicationUserId
                );

                var order = new Order
                {
                    CustomerId = customer.Id,
                    OrderStatus = OrderStatus.Pending,
                    OrderDate = DateTime.UtcNow,
                };

                decimal totalAmount = 0;

                foreach (var orderItemDto in orderDto.OrderItems)
                {
                    var product = await productService.GetProductAsync(orderItemDto.productId);

                    var inventoryReservations = await inventoryService.ReserveInventoryAsync(
                        product.Id,
                        orderItemDto.Quantity,
                        cancellationToken
                    );

                    foreach (var reservation in inventoryReservations)
                    {
                        order.Reservations.Add(
                            new Reservation
                            {
                                ProductId = product.Id,
                                WarehouseId = reservation.WarehouseId,
                                ReservedQuantity = reservation.ReservedQuantity,
                                ExpiresAt = DateTime.UtcNow.AddMinutes(15),
                            }
                        );
                    }

                    var newOrderItem = new OrderItem
                    {
                        ProductId = product.Id,
                        Quantity = orderItemDto.Quantity,
                        UnitPrice = product.UnitPrice,
                    };

                    order.OrderItems.Add(newOrderItem);
                    totalAmount += product.UnitPrice * orderItemDto.Quantity;
                }

                order.TotalAmount = totalAmount;

                dbContext.Orders.Add(order);
                await dbContext.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                logger.LogInformation(
                    "Order created with ID {OrderId} for Customer {CustomerId} with total amount {TotalAmount}",
                    order.Id,
                    order.CustomerId,
                    order.TotalAmount
                );
                var orderResponse = OrderResponseFactory.FromOrder(order);
                return orderResponse;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public async Task ExpireReservationsAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;

            var expiredReservations = await dbContext
                .Reservations.Include(reservation => reservation.Order)
                .Where(reservation =>
                    reservation.ExpiresAt < now
                    && reservation.Order.OrderStatus != OrderStatus.Paid
                    && reservation.Order.OrderStatus != OrderStatus.Cancelled
                )
                .ToListAsync(cancellationToken);

            if (expiredReservations.Count == 0)
            {
                return;
            }

            var expiredOrderIds = expiredReservations
                .Select(reservation => reservation.OrderId)
                .Distinct()
                .ToList();

            foreach (var orderId in expiredOrderIds)
            {
                await using var transaction = await dbContext.Database.BeginTransactionAsync(
                    cancellationToken
                );

                try
                {
                    var orderReservations = expiredReservations
                        .Where(reservation => reservation.OrderId == orderId)
                        .ToList();

                    var reservationGroups = orderReservations.GroupBy(reservation => new
                    {
                        reservation.WarehouseId,
                        reservation.ProductId,
                    });

                    foreach (var group in reservationGroups)
                    {
                        var totalToRelease = group.Sum(reservation => reservation.ReservedQuantity);

                        await dbContext
                            .Inventories.Where(inv =>
                                inv.WarehouseId == group.Key.WarehouseId
                                && inv.ProductId == group.Key.ProductId
                            )
                            .ExecuteUpdateAsync(
                                setters =>
                                    setters
                                        .SetProperty(
                                            inv => inv.AvailableQuantity,
                                            inv => inv.AvailableQuantity + totalToRelease
                                        )
                                        .SetProperty(
                                            inv => inv.ReservedQuantity,
                                            inv => inv.ReservedQuantity - totalToRelease
                                        )
                                        .SetProperty(inv => inv.LastReservedAt, now),
                                cancellationToken
                            );
                    }

                    var order = await dbContext
                        .Orders.Where(order => order.Id == orderId)
                        .ExecuteUpdateAsync(
                            setters =>
                                setters.SetProperty(
                                    order => order.OrderStatus,
                                    OrderStatus.Cancelled
                                ),
                            cancellationToken
                        );

                    dbContext.Reservations.RemoveRange(orderReservations);
                    await dbContext.SaveChangesAsync(cancellationToken);

                    await transaction.CommitAsync(cancellationToken);

                    logger.LogInformation(
                        "Order {OrderId} expired. {ReservationCount} reservation(s) released.",
                        orderId,
                        orderReservations.Count
                    );
                }
                catch
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
            }
        }

        public async Task<Order> GetOrderByIdAsync(
            Guid orderId,
            CancellationToken cancellationToken = default
        )
        {
            var order = await dbContext
                .Orders.AsNoTracking()
                .Include(order => order.OrderItems)
                .Include(order => order.Reservations)
                .FirstOrDefaultAsync(order => order.Id == orderId, cancellationToken);

            if (order == null)
            {
                logger.LogError("Order with ID {OrderId} not found", orderId);
                throw new NotFoundException("Order not found");
            }
            return order;
        }

        public async Task MarkOrderAsPaidAsync(
            Guid orderId,
            CancellationToken cancellationToken = default
        )
        {
            var affectedRows = await dbContext
                .Orders.Where(order => order.Id == orderId)
                .ExecuteUpdateAsync(
                    setters => setters.SetProperty(order => order.OrderStatus, OrderStatus.Paid),
                    cancellationToken
                );

            if (affectedRows == 0)
            {
                logger.LogError("Order with ID {OrderId} not found", orderId);
                throw new NotFoundException("Order not found");
            }
        }
    }
}
