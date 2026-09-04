using WarehouseFlow.Domain.Entities;

namespace WarehouseFlow.Application.Interfaces;

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(Guid customerId, CancellationToken cancellationToken = default);
    Task<Customer?> GetByApplicationUserIdAsync(
        string applicationUserId,
        CancellationToken cancellationToken = default
    );
    Task<bool> ExistsAsync(Guid customerId, CancellationToken cancellationToken = default);
}
