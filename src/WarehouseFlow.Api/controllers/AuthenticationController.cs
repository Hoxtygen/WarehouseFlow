using Microsoft.AspNetCore.Mvc;
using WarehouseFlow.Api.Contracts;
using WarehouseFlow.Api.Controllers;
using WarehouseFlow.Application.Dtos;
using WarehouseFlow.Application.Interfaces;

public class AuthenticationController(IAuthenticationService authenticationService) : BaseController
{
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<CreatedUserResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register(CreateUserDto userDto)
    {
        var result = await authenticationService.AddUserAsync(userDto);
        return Created(
            result,
            nameof(Register),
            new { id = result.Id },
            "User registered successfully."
        );
    }
}
