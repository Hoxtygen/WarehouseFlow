using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WarehouseFlow.Application.Interfaces;
using WarehouseFlow.Domain.Entities;
using WarehouseFlow.Domain.Exceptions;
using WarehouseFlow.Infrastructure.Data;

namespace WarehouseFlow.Infrastructure.Implementations;

public sealed class CustomerService(AppDbContext dbContext, ILogger<CustomerService> logger)
    : ICustomerService
{
    public async Task<Customer> GetCustomerAsync(Guid customerId)
    {
        var customer = await dbContext
            .Customers.AsNoTracking()
            .FirstOrDefaultAsync(existingCustomer => existingCustomer.Id == customerId);

        if (customer is null)
        {
            logger.LogError("Customer with ID {CustomerId} not found", customerId);
            throw new NotFoundException($"Customer with ID {customerId} not found");
        }

        return customer;
    }

    public async Task<Customer> GetCustomerByApplicationUserIdAsync(string applicationUserId)
    {
        var customer = await dbContext
            .Customers.AsNoTracking()
            .FirstOrDefaultAsync(existingCustomer => existingCustomer.ApplicationUserId == applicationUserId);

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
