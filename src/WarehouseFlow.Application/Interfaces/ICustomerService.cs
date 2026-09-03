using WarehouseFlow.Domain.Entities;

namespace WarehouseFlow.Application.Interfaces;

public interface ICustomerService
{
    Task<Customer> GetCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);

    Task<Customer> GetCustomerByApplicationUserIdAsync(
        string applicationUserId,
        CancellationToken cancellationToken = default
    );
}
