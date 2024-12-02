using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Alexandria.Api.Common.Middleware;

public class EndpointInitializationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var user = context.User;
        if (user.Identity?.IsAuthenticated == true)
        {
            var subClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (Guid.TryParse(subClaim, out var userId))
            {
                EndpointBase.InitializeUserId(userId);
                context.Items["UserId"] = userId;
            }
        }

        await next(context);
    }
}