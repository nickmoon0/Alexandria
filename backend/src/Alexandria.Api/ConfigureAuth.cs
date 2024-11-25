using Alexandria.Api.Common.Roles;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Alexandria.Api;

public static class ConfigureAuth
{
    public static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .SetupAuthentication(configuration)
            .SetupAuthorization();
    }

    private static IServiceCollection SetupAuthorization(this IServiceCollection services)
    {
        services.AddAuthorizationBuilder()
            .AddPolicy(nameof(Admin), policy => policy.RequireRole(nameof(Admin)))
            .AddPolicy(nameof(User), policy => 
                policy.RequireRole(nameof(User), nameof(Admin)));
        
        return services;
    }
    
    private static IServiceCollection SetupAuthentication(this IServiceCollection services,
        IConfiguration configuration)
    {
        // Get configuration parameters
        var authority = configuration["IDP:Authority"] 
                        ?? throw new Exception("Authority cannot be null");
        var audience = configuration["IDP:Audience"] 
                       ?? throw new Exception("Audience cannot be null");
        var requireHttpsMetadata = Convert.ToBoolean(configuration["IDP:RequireHttpsMetadata"] ?? false.ToString());
        
        // Setup AuthN
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(jwtOptions =>
            {
                jwtOptions.Authority = authority;
                jwtOptions.Audience = audience;
                jwtOptions.RequireHttpsMetadata = requireHttpsMetadata;
                jwtOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                };
            });
        return services;
    }
}