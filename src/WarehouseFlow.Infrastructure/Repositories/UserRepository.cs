using Microsoft.EntityFrameworkCore;
using WarehouseFlow.Application.Dtos;
using WarehouseFlow.Domain.Entities;
using WarehouseFlow.Domain.Interfaces;
using WarehouseFlow.Infrastructure.Data;

namespace WarehouseFlow.Infrastructure.Repositories;

public class UserRepository : Repository<User>, IUserRepositoryInterface
{
    public UserRepository(AppDbContext context)
        : base(context) { }

         public async Task AddUserAsync(User user)
    {
        await _dbset.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _dbset.AnyAsync(u => u.Email == email);
    }

    public async Task<bool> PhoneNumberExistsAsync(string phoneNumber)
    {
        return await _dbset.AnyAsync(u => u.PhoneNumber == phoneNumber);
    }


    public Task<User?> GetUserByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }

    public Task<User?> GetUserByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<User>> GetAllUsersAsync()
    {
        throw new NotImplementedException();
    }

    public Task UpdateUserAsync(User user)
    {
        throw new NotImplementedException();
    }

    public Task DeleteUserAsync(User user)
    {
        throw new NotImplementedException();
    }
}
