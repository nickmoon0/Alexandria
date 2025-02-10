using Alexandria.Application.Common.Interfaces;

namespace Alexandria.FileApi.Common.Middleware;

public class TokenAuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public TokenAuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITokenService tokenService)
    {
        var token = context.Request.Query["token"].ToString();

        if (string.IsNullOrEmpty(token) || !tokenService.ValidateToken(token, out var documentId, out var filePermissions))
        {
            // No message for security
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        // DocumentId and FilePermissions will never be null if token valid
        EndpointBase.InitializeTokenParameters((Guid)documentId!, filePermissions!);
        context.Items["DocumentId"] = documentId;
        context.Items["FilePermissions"] = filePermissions;

        await _next(context);
    }
}