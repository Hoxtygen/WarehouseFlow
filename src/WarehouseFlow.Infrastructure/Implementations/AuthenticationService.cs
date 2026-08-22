using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WarehouseFlow.Application.Dtos;
using WarehouseFlow.Application.Interfaces;
using WarehouseFlow.Domain.Entities;
using WarehouseFlow.Domain.Enum;
using WarehouseFlow.Domain.Exceptions;
using WarehouseFlow.Domain.ValueObjects;
using WarehouseFlow.Infrastructure.Data;

namespace WarehouseFlow.Infrastructure.Implementations;

public sealed class AuthenticationService(
    UserManager<ApplicationUser> userManager,
    AppDbContext dbContext,
    ITokenService tokenService,
    ILogger<AuthenticationService> _logger
) : IAuthenticationService
{
    // private readonly ILogger<AuthenticationService> _logger;
    public async Task<LoginResponse> LoginAsync(
        LoginRequest request,
        CancellationToken cancellationToken = default
    )
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
        {
            _logger.LogError("Invalid user login. Email:{Email}", request.Email);
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        if (await userManager.IsLockedOutAsync(user))
        {
            _logger.LogError(
                "User login attempt for locked out user. Email:{Email}",
                request.Email
            );
            throw new UnauthorizedAccessException("This account is locked.");
        }

        var roleNames = await userManager.GetRolesAsync(user);
        var roleName = roleNames.FirstOrDefault() ?? string.Empty;

        _logger.LogInformation("Successful user login for user:{Email}", request.Email);

        return new LoginResponse
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email!,
            Role = roleName,
            AccessToken = tokenService.GenerateToken(
                user.Id,
                user.Email!,
                user.UserName ?? user.Email!,
                roleName
            ),
        };
    }

    public async Task<CreatedUserResponse> RegisterCustomerAsync(
        RegisterCustomerDto registration,
        CancellationToken cancellationToken = default
    )
    {
        await EnsurePhoneNumberIsAvailableAsync(registration.PhoneNumber, cancellationToken);

        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            cancellationToken
        );

        var user = new ApplicationUser
        {
            UserName = registration.Email,
            Email = registration.Email,
            PhoneNumber = registration.PhoneNumber,
            FirstName = registration.FirstName,
            LastName = registration.LastName,
        };

        var createResult = await userManager.CreateAsync(user, registration.Password);
        EnsureUserCreationSucceeded(createResult);

        var roleName = UserRole.Customer.ToString();
        var roleResult = await userManager.AddToRoleAsync(user, roleName);
        EnsureRoleAssignmentSucceeded(roleResult, roleName);

        var customer = new Customer(
            registration.FirstName,
            registration.LastName,
            registration.Email,
            registration.PhoneNumber,
            new Address(
                registration.Address.Street,
                registration.Address.HouseNumber,
                registration.Address.City,
                registration.Address.State,
                registration.Address.LandMark
            )
        )
        {
            ApplicationUserId = user.Id,
        };

        dbContext.Customers.Add(customer);
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new CreatedUserResponse
        {
            Id = customer.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email!,
            PhoneNumber = user.PhoneNumber!,
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt ?? customer.CreatedAt,
            role = UserRole.Customer,
        };
    }

    public async Task<CreatedUserResponse> RegisterEmployeeAsync(
        CreateEmployeeUserDto employeeUserDto,
        string createdByUserId,
        CancellationToken cancellationToken = default
    )
    {
        await EnsurePhoneNumberIsAvailableAsync(employeeUserDto.PhoneNumber, cancellationToken);

        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            cancellationToken
        );

        var employee = new ApplicationUser
        {
            UserName = employeeUserDto.Email,
            Email = employeeUserDto.Email,
            PhoneNumber = employeeUserDto.PhoneNumber,
            FirstName = employeeUserDto.FirstName,
            LastName = employeeUserDto.LastName,
        };

        IdentityResult? result = await userManager.CreateAsync(employee, employeeUserDto.Password);
        _logger.LogInformation("User created succesfully: {Email}", employee.Email);
        EnsureUserCreationSucceeded(result);

        var roleName = ValidateEmployeeRole(employeeUserDto.Role);
        var roleResult = await userManager.AddToRoleAsync(employee, roleName);
        EnsureRoleAssignmentSucceeded(roleResult, roleName);

        var newEmployee = new Employee(
            employeeUserDto.EmployeeNumber,
            employeeUserDto.Address,
            roleName,
            employee.Id,
            createdByUserId
        );

        dbContext.Employees.Add(newEmployee);
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return new CreatedUserResponse
        {
            Id = newEmployee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email!,
            PhoneNumber = employee.PhoneNumber!,
            CreatedAt = newEmployee.CreatedAt,
            UpdatedAt = newEmployee.UpdatedAt ?? newEmployee.CreatedAt,
            role = Enum.Parse<UserRole>(roleName, true),
        };
    }

    private static string ValidateEmployeeRole(string role)
    {
        if (
            !Enum.TryParse<UserRole>(role, true, out var parsedRole)
            || parsedRole == UserRole.Customer
        )
        {
            throw new ArgumentException("A valid employee role is required.", nameof(role));
        }

        return parsedRole.ToString();
    }

    private async Task EnsurePhoneNumberIsAvailableAsync(
        string phoneNumber,
        CancellationToken cancellationToken
    )
    {
        if (
            await dbContext.Users.AnyAsync(
                user => user.PhoneNumber == phoneNumber,
                cancellationToken
            )
        )
        {
            throw new DuplicateException("A user with this phone number already exists.");
        }
    }

    private static void EnsureUserCreationSucceeded(IdentityResult result)
    {
        if (!result.Succeeded)
        {
            var duplicateError = result.Errors.FirstOrDefault(error =>
                error.Code is "DuplicateUserName" or "DuplicateEmail"
            );

            if (duplicateError is not null)
            {
                throw new DuplicateException("A user with this email already exists.");
            }

            throw new ValidationException(
                "Could not create the user account.",
                result.Errors.Select(error => error.Description)
            );
        }
    }

    private static void EnsureRoleAssignmentSucceeded(IdentityResult result, string roleName)
    {
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(error => error.Description));
            throw new InvalidOperationException($"Could not assign the '{roleName}' role. {errors}");
        }
    }
}
