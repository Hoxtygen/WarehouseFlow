namespace WarehouseFlow.Application.Dtos;

public record PaymentResult(
    string Message,
    Guid paymentId,
    Guid orderId
);