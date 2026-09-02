
namespace WarehouseFlow.Application.Dtos;
public record PaymentDto(
    Guid OrderId,
    decimal Amount)
{
}