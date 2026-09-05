namespace WarehouseFlow.Api.Middleware;

public sealed class SecurityHeadersMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        context.Response.Headers.Remove("Server");
        context.Response.Headers.Remove("X-Powered-By");

        context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Append("X-Frame-Options", "DENY");
        context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
        context.Response.Headers.Append("Content-Security-Policy", "default-src 'none'; frame-ancestors 'none';");
        context.Response.Headers.Append("Strict-Transport-Security", "max-age=31536000; includeSubDomains; preload");

        await next(context);
    }
}
