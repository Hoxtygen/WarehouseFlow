using Microsoft.AspNetCore.Mvc;
using WarehouseFlow.Api.Contracts;

namespace WarehouseFlow.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public abstract class BaseController : ControllerBase
{
    protected IActionResult Success<T>(
        T data,
        string? message = null,
        int statusCode = StatusCodes.Status200OK
    )
    {
        return StatusCode(statusCode, ApiResponse<T>.SuccessResult(data, message, statusCode));
    }

    protected IActionResult Created<T>(
        T data,
        string actionName,
        object routeValues,
        string? message = null
    )
    {
        var response = ApiResponse<T>.CreatedResult(data, message);
        return CreatedAtAction(actionName, routeValues, response);
    }

    protected IActionResult Failure(
        string message,
        int statusCode,
        IEnumerable<string>? errors = null
    )
    {
        return StatusCode(
            statusCode,
            ApiResponse<object>.FailureResult(message, errors, statusCode)
        );
    }
}
