using Microsoft.Extensions.Logging;
using WarehouseFlow.Application.Interfaces;
using WarehouseFlow.Domain.Entities;
using WarehouseFlow.Domain.Exceptions;

namespace WarehouseFlow.Application.Services;

public sealed class CustomerService(
    ICustomerRepository customerRepository,
    ILogger<CustomerService> logger
) : ICustomerService
{
    public async Task<Customer> GetCustomerAsync(
        Guid customerId,
        CancellationToken cancellationToken
    )
    {
        var customer = await customerRepository.GetByIdAsync(customerId, cancellationToken);

        if (customer is null)
        {
            logger.LogError("Customer with ID {CustomerId} not found", customerId);
            throw new NotFoundException($"Customer with ID {customerId} not found");
        }

        return customer;
    }

    public async Task<Customer> GetCustomerByApplicationUserIdAsync(
        string applicationUserId,
        CancellationToken cancellationToken
    )
    {
        var customer = await customerRepository.GetByApplicationUserIdAsync(
            applicationUserId,
            cancellationToken
        );

        if (customer is null)
        {
            logger.LogError(
                "Customer with ApplicationUserId {ApplicationUserId} not found",
                applicationUserId
            );
            throw new NotFoundException(
                $"Customer with ApplicationUserId {applicationUserId} not found"
            );
        }

        return customer;
    }
}
