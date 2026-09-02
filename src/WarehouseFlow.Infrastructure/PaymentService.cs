using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WarehouseFlow.Application.Dtos;
using WarehouseFlow.Application.Interfaces;
using WarehouseFlow.Domain.Entities;
using WarehouseFlow.Domain.Enum;
using WarehouseFlow.Infrastructure.Data;

namespace WarehouseFlow.Infrastructure.Implementations;

public sealed class PaymentService(
    ICustomerService customerService,
    IOrderService orderService,
    IInventoryService inventoryService,
    AppDbContext dbContext,
    ILogger<PaymentService> logger
) : IPaymentService
{
    public async Task<PaymentResult> ProcessPaymentAsync(
        PaymentDto paymentDto,
        Guid applicationUserId,
        CancellationToken cancellationToken = default
    )
    {
        var customer = await customerService.GetCustomerAsync(applicationUserId);
        var order = await orderService.GetOrderByIdAsync(paymentDto.OrderId, cancellationToken);

        if (order.TotalAmount != paymentDto.Amount)
        {
            logger.LogError(
                "Payment amount {Amount} does not match order total {OrderTotal} for Order {OrderId}",
                paymentDto.Amount,
                order.TotalAmount,
                paymentDto.OrderId
            );
            throw new InvalidOperationException("Payment amount does not match order total");
        }

        if (customer.Id != order.CustomerId)
        {
            logger.LogError(
                "Customer {CustomerId} is not the owner of Order {OrderId}",
                applicationUserId,
                paymentDto.OrderId
            );
            throw new InvalidOperationException("Customer is not the owner of the order");
        }

        if (order.OrderStatus != OrderStatus.Pending)
        {
            logger.LogError(
                "Order {OrderId} is not pending. Current status: {OrderStatus}",
                paymentDto.OrderId,
                order.OrderStatus
            );
            throw new InvalidOperationException("Only pending orders can be paid");
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            cancellationToken
        );

        try
        {
            foreach (var reservation in order.Reservations)
            {
                var inventory = await inventoryService.GetInventoryByProductAndWarehouseAsync(
                    reservation.ProductId,
                    reservation.WarehouseId,
                    cancellationToken
                );

                inventory.ReservedQuantity -= reservation.ReservedQuantity;
            }

            await orderService.MarkOrderAsPaidAsync(paymentDto.OrderId, cancellationToken);

            var payment = new Payment
            {
                OrderId = paymentDto.OrderId,
                CustomerId = customer.Id,
                Amount = paymentDto.Amount,
            };
            dbContext.Payments.Add(payment);
            await dbContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            logger.LogInformation(
                "Payment of {Amount} processed for Order {OrderId} by Customer {CustomerId}",
                paymentDto.Amount,
                paymentDto.OrderId,
                customer.Id
            );

            return new PaymentResult(
                "Payment processed successfully",
                payment.Id,
                paymentDto.OrderId
            );
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
