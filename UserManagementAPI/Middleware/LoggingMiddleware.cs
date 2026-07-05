using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Log Request
        _logger.LogInformation("➡️ Request: {method} {path}",
            context.Request.Method,
            context.Request.Path);

        await _next(context);
        if (context.Response.StatusCode>=400)
        {
            _logger.LogWarning("⚠️ Response: {statusCode}",
                context.Response.StatusCode);
        }
        else
        {
            _logger.LogInformation("⬅️ Response: {statusCode}",
                context.Response.StatusCode);
        }

      
    }
}
