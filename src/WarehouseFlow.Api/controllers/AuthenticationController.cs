using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using WarehouseFlow.Api.Contracts;
using WarehouseFlow.Application.Dtos;
using WarehouseFlow.Application.Interfaces;

namespace WarehouseFlow.Api.Controllers;

[Route("api/v1/auth")]
[EnableRateLimiting("auth")]
public class AuthenticationController(IAuthenticationService authenticationService) : BaseController
{
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<LoginResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
       [FromBody] LoginRequest request,
        CancellationToken cancellationToken
    )
    {
        var result = await authenticationService.LoginAsync(request, cancellationToken);
        return Success(result, "Login successful.");
    }

    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<CreatedUserResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register(
       [FromBody] RegisterCustomerDto registration,
        CancellationToken cancellationToken
    )
    {
        var result = await authenticationService.RegisterCustomerAsync(
            registration,
            cancellationToken
        );

        return Created(
            result,
            nameof(Register),
            new { id = result.Id },
            "Customer registered successfully."
        );
    }

    [HttpPost("employee/register")]
    [Authorize(Roles = "Super_Admin")]
    [ProducesResponseType(typeof(ApiResponse<CreatedUserResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RegisterEmployee(
      [FromBody]  CreateEmployeeUserDto employeeUserDto,
        CancellationToken cancellationToken
    )
    {
        var createdByUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(createdByUserId))
        {
            return Problem(
                title: "The authenticated user ID is missing from the access token.",
                statusCode: StatusCodes.Status401Unauthorized
            );
        }

        var result = await authenticationService.RegisterEmployeeAsync(
            employeeUserDto,
            createdByUserId,
            cancellationToken
        );

        return Created(
            result,
            nameof(RegisterEmployee),
            new { id = result.Id },
            "Employee created successfully"
        );
    }
}
