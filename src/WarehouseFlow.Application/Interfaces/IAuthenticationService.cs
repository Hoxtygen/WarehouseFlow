using WarehouseFlow.Application.Dtos;
using WarehouseFlow.Domain.Entities;

namespace WarehouseFlow.Application.Interfaces;
public interface IAuthenticationService
{
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> GetUserByIdAsync(Guid id);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<CreatedUserResponse> AddUserAsync(CreateUserDto user);
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync(User user);
}
