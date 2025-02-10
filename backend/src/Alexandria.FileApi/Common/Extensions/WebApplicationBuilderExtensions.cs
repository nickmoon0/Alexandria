using Alexandria.FileApi.Common.Middleware;

namespace Alexandria.FileApi.Common.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static IApplicationBuilder UseTokenAuthentication(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TokenAuthenticationMiddleware>();
    }
}