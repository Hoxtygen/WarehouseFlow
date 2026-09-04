using WarehouseFlow.Application.Dtos;

namespace WarehouseFlow.Application.Interfaces;

public interface IPaymentService
{
    Task<PaymentResult> ProcessPaymentAsync(
        PaymentDto paymentDto,
        string applicationUserId,
        CancellationToken cancellationToken = default
    );
}
