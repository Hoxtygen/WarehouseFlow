using Microsoft.Extensions.Logging;
using WarehouseFlow.Application.Dtos;
using WarehouseFlow.Application.Interfaces;
using WarehouseFlow.Domain.Entities;
using WarehouseFlow.Domain.Enum;

namespace WarehouseFlow.Application.Services;

public sealed class PaymentService(
    IPaymentRepository paymentRepository,
    ICustomerService customerService,
    IOrderService orderService,
    IInventoryService inventoryService,
    IUnitOfWork unitOfWork,
    ILogger<PaymentService> logger
) : IPaymentService
{
    public async Task<PaymentResult> ProcessPaymentAsync(
        PaymentDto paymentDto,
        string applicationUserId,
        CancellationToken cancellationToken = default
    )
    {
        var customer = await customerService.GetCustomerByApplicationUserIdAsync(applicationUserId);
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

        await unitOfWork.BeginTransactionAsync(cancellationToken);

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
            await paymentRepository.AddAsync(payment);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            await unitOfWork.CommitAsync(cancellationToken);

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
            await unitOfWork.RollbackAsync(cancellationToken);
            throw;
        }
    }
}
