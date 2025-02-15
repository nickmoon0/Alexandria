using System.Security.Claims;
using System.Text.Json;
using Alexandria.Application.Common.Interfaces;
using Alexandria.Application.Common.Roles;

namespace Alexandria.CoreApi.Common.Middleware;

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
            
            var roleFactory = context.RequestServices.GetRequiredService<IRoleFactory>();
            var userRoles = user
                .FindAll(ClaimTypes.Role)
                .Select(claim => roleFactory.CreateRoleInstance(claim.Value))
                .OfType<Role>()
                .ToList();
            
            // Initialize the roles in EndpointBase
            EndpointBase.InitializeRoles(userRoles);
            context.Items["UserRoles"] = userRoles;
        }

        await next(context);
    }
}