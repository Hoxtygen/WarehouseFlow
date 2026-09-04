using Microsoft.EntityFrameworkCore;
using WarehouseFlow.Application.Interfaces;
using WarehouseFlow.Domain.Entities;
using WarehouseFlow.Infrastructure.Data;

namespace WarehouseFlow.Infrastructure.Repositories;

public class CustomerRepository(AppDbContext dbContext) : ICustomerRepository
{
    public async Task<Customer?> GetByIdAsync(
        Guid customerId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext.Customers.AsNoTracking().FirstOrDefaultAsync(
            customer => customer.Id == customerId,
            cancellationToken
        );
    }

    public async Task<Customer?> GetByApplicationUserIdAsync(
        string applicationUserId,
        CancellationToken cancellationToken = default
    )
    {
        return await dbContext.Customers.AsNoTracking().FirstOrDefaultAsync(
            customer => customer.ApplicationUserId == applicationUserId,
            cancellationToken
        );
    }

    public async Task<bool> ExistsAsync(Guid customerId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Customers.AnyAsync(
            customer => customer.Id == customerId,
            cancellationToken
        );
    }
}
