using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WarehouseFlow.Domain.Exceptions;

namespace WarehouseFlow.Api.Middleware;

public class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken
    )
    {
        logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

        HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
        string message = "An error occurred while processing your request.";
        IEnumerable<string>? errors = null;
        switch (exception)
        {
            case ValidationException validationException:
                statusCode = HttpStatusCode.BadRequest;
                message = validationException.Message;
                errors = validationException.Errors;
                break;
            case UnauthorizedAccessException:
                statusCode = HttpStatusCode.Unauthorized;
                message = exception.Message;
                break;
            case DuplicateException:
                statusCode = HttpStatusCode.Conflict;
                message = exception.Message;
                break;

            case NotFoundException:
                statusCode = HttpStatusCode.NotFound;
                message = exception.Message;
                break;

            case InsufficientStockException:
                statusCode = HttpStatusCode.BadRequest;
                message = exception.Message;
                break;
        }

        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = message,
            Type = $"https://httpstatuses.com/{(int)statusCode}",
        };

        if (errors is not null)
        {
            problemDetails.Extensions["errors"] = errors;
        }

        httpContext.Response.StatusCode = (int)statusCode;
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }
}
