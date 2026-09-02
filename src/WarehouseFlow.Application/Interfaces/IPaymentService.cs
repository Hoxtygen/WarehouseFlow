using WarehouseFlow.Application.Dtos;

namespace WarehouseFlow.Application.Interfaces;

public interface IPaymentService
{
    Task<PaymentResult> ProcessPaymentAsync(
        PaymentDto paymentDto,
        Guid applicationUserId,
        CancellationToken cancellationToken = default
    );
}
