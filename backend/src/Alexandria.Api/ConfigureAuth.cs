using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Alexandria.Api;

public static class ConfigureAuth
{
    public static IServiceCollection AddAuth(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Get configuration parameters
        var authority = configuration["IDP:Authority"] 
                        ?? throw new Exception("Authority cannot be null");
        var audience = configuration["IDP:Audience"] 
                       ?? throw new Exception("Audience cannot be null");
        var requireHttpsMetadata = Convert.ToBoolean(configuration["IDP:RequireHttpsMetadata"] ?? false.ToString());
        
        // Enabled AuthN/Z services
        services.AddAuthorization();
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