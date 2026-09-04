using Microsoft.Extensions.Logging;
using WarehouseFlow.Application.Dtos;
using WarehouseFlow.Application.Interfaces;
using WarehouseFlow.Domain.Entities;
using WarehouseFlow.Domain.Enum;
using WarehouseFlow.Domain.Exceptions;

namespace WarehouseFlow.Application.Services;

public sealed class OrderService(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork,
    ICustomerService customerService,
    IProductService productService,
    IInventoryService inventoryService,
    ILogger<OrderService> logger
) : IOrderService
{
    public async Task<OrderResponse> CreateOrder(
        OrderDto orderDto,
        string applicationUserId,
        CancellationToken cancellationToken
    )
    {
        await unitOfWork.BeginTransactionAsync(cancellationToken);
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
                var product = await productService.GetProductAsync(orderItemDto.ProductId);

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
            await orderRepository.AddAsync(order);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            await unitOfWork.CommitAsync(cancellationToken);

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
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }

    public async Task ExpireReservationsAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var expiredReservations = await orderRepository.GetExpiredReservationsAsync(
            now,
            cancellationToken
        );

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
            await unitOfWork.BeginTransactionAsync(cancellationToken);

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

                    await inventoryService.ReleaseReservedStockAsync(
                        group.Key.WarehouseId,
                        group.Key.ProductId,
                        totalToRelease,
                        now,
                        cancellationToken
                    );
                }

             var markOrderAsCancelled =    await orderRepository.MarkAsCancelledAsync(orderId, cancellationToken);
             if (!markOrderAsCancelled)
             {
                logger.LogError("Order with id {OrderId} failed to be cancelled", orderId);
                throw new  InvalidOperationException($"Order with id {orderId} failed to be cancelled");
             }

                await orderRepository.DeleteReservationsAsync(orderReservations);
                await unitOfWork.SaveChangesAsync(cancellationToken);

                await unitOfWork.CommitAsync(cancellationToken);

                logger.LogInformation(
                    "Order {OrderId} expired. {ReservationCount} reservation(s) released.",
                    orderId,
                    orderReservations.Count
                );
            }
            catch
            {
                await unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }

    public async Task<Order> GetOrderByIdAsync(
        Guid orderId,
        CancellationToken cancellationToken = default
    )
    {
        var order = await orderRepository.GetByIdWithDetailsAsync(orderId, cancellationToken);

        if (order == null)
        {
            logger.LogError("Order with ID {OrderId} not found", orderId);
            throw new NotFoundException("Order not found");
        }
        return order;
    }

    public async Task MarkOrderAsPaidAsync(Guid orderId, CancellationToken cancellationToken)
    {
        var wasPaid = await orderRepository.MarkAsPaidAsync(orderId, cancellationToken);

        if (!wasPaid)
        {
            logger.LogError("Order with ID {OrderId} not found", orderId);
            throw new NotFoundException("Order not found");
        }
    }
}
