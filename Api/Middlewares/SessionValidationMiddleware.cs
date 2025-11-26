using Api.Database;
using System;
using System.Security.Claims;

namespace Api.Middlewares;

public class SessionValidationMiddleware
{
    private readonly RequestDelegate _next;

    public SessionValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, DatabaseContext db)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userId = int.TryParse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var id) ? id : (int?)null;
            var tokenSessionId = context.User.FindFirst("sessionId")?.Value;

            if (userId != null && tokenSessionId != null)
            {
                var user = await db.Users.FindAsync(userId);

                if (user == null || user.SessionId?.ToString() != tokenSessionId)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Sesión expirada o iniciada en otro dispositivo");
                    return;
                }
            }
        }

        await _next(context);
    }
}
