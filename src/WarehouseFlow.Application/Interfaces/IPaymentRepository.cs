using WarehouseFlow.Domain.Entities;

namespace WarehouseFlow.Application.Interfaces;

public interface IPaymentRepository
{
    Task AddAsync(Payment payment, CancellationToken cancellationToken = default);
}
