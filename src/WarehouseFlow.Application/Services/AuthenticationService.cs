using Microsoft.Extensions.Logging;
using WarehouseFlow.Application.Dtos;
using WarehouseFlow.Application.Interfaces;
using WarehouseFlow.Domain.Entities;
using WarehouseFlow.Domain.Exceptions;
using WarehouseFlow.Domain.Interfaces;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepositoryInterface _userRepository;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(
        IUserRepositoryInterface userRepository,
        ILogger<AuthenticationService> logger
    )
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<CreatedUserResponse> AddUserAsync(CreateUserDto userDto)
    {
        bool emailExists = await _userRepository.EmailExistsAsync(userDto.Email);
        if (emailExists)
        {
            _logger.LogError(
                "Duplicate user with email: {Email} registration attempt",
                userDto.Email
            );
            throw new DuplicateException("Email already exists.");
        }

        bool phoneNumberExists = await _userRepository.PhoneNumberExistsAsync(userDto.PhoneNumber);
        if (phoneNumberExists)
        {
            _logger.LogError(
                "Duplicate user with phone number: {PhoneNumber} registration attempt",
                userDto.PhoneNumber
            );
            throw new DuplicateException("Phone number already exists.");
        }

        string hashedPassword = HashPassword(userDto.Password);
        var user = new User(
            userDto.FirstName,
            userDto.LastName,
            ToLowerCase(userDto.Email),
            userDto.PhoneNumber,
            hashedPassword
        );

        await _userRepository.AddUserAsync(user);
        _logger.LogInformation("Registering new user with email: {Email}", userDto.Email);
        return new CreatedUserResponse
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            CreatedAt = user.CreatedAt,
            UpdatedAt = (DateTime)user.UpdatedAt,
            role = user.Role,
        };
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _userRepository.GetUserByEmailAsync(email);
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await _userRepository.GetUserByIdAsync(id);
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllUsersAsync();
    }

    public async Task UpdateUserAsync(User user)
    {
        await _userRepository.UpdateUserAsync(user);
    }

    public async Task DeleteUserAsync(User user)
    {
        await _userRepository.DeleteUserAsync(user);
    }

    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    private string ToLowerCase(string email)
    {
        return email.ToLower();
    }
}
