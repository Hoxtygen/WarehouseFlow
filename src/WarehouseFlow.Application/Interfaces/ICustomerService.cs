using WarehouseFlow.Domain.Entities;

namespace WarehouseFlow.Application.Interfaces;

public interface ICustomerService
{
    Task<Customer> GetCustomerAsync(Guid customerId);

    Task<Customer> GetCustomerByApplicationUserIdAsync(string applicationUserId);
}
