using Alexandria.Application.Common.Interfaces;
using Alexandria.Domain.Common.Interfaces;
using Alexandria.Infrastructure.Services;

namespace Alexandria.FileApi;

public static class ConfigureServices
{
    public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenApi();

        builder.Services.AddScoped<IDateTimeProvider, SystemDateTimeProvider>();
        builder.Services.AddScoped<ITokenService, TokenService>();
        
        return builder;
    }
}