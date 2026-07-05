using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

public class AuthenticationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuthenticationMiddleware> _logger;

    public AuthenticationMiddleware(
        RequestDelegate next,
        ILogger<AuthenticationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Récupération du token dans l'en-tête Authorization
        var authHeader = context.Request.Headers["Authorization"].ToString();

        if (string.IsNullOrWhiteSpace(authHeader) || !IsTokenValid(authHeader))
        {
            _logger.LogWarning("Unauthorized request: missing or invalid token.");

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync("{\"error\": \"Unauthorized\"}");
            return;
        }

        await _next(context);
    }

    private bool IsTokenValid(string authHeader)
    {
            return authHeader == "SecretToken"; 
    }
}
