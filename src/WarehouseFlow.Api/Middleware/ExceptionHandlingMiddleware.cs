using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using WarehouseFlow.Api.Contracts;
using WarehouseFlow.Domain.Exceptions;

namespace WarehouseFlow.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger
    )
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
        string message = "An error occurred while processing your request.";
        // var errors = new[] { exception.Message };
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

        var response = ApiResponse<object>.FailureResult(message, errors, (int)statusCode);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        return context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
    }
}
