using Alexandria.Api.Common.Roles;
using Alexandria.Application;
using Alexandria.Infrastructure;
using Alexandria.Infrastructure.Common.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.IdentityModel.Tokens;

namespace Alexandria.Api;

public static class ConfigureServices
{
    public const string LocalHost5173CorsPolicy = nameof(LocalHost5173CorsPolicy);
    public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(LocalHost5173CorsPolicy, corsBuilder =>
            {
                corsBuilder.WithOrigins("http://localhost:5173")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
        builder.Services.AddOpenApi();
        builder.WebHost.ConfigureKestrel(options =>
        {
            // Set max request body size for all endpoints
            options.Limits.MaxRequestBodySize = 16106127360; // 15 GB
        });
        
        builder.Services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = 16106127360; // 15 GB
        });
        
        builder.Services.AddAntiforgery();
        
        builder.Services.AddAuth(builder.Configuration);

        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration);
        
        builder.AddOptions();
        
        return builder;
    }

    /**
     * Options
     */
    
    private static WebApplicationBuilder AddOptions(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<RabbitMqOptions>(
            builder.Configuration.GetSection(nameof(RabbitMqOptions)));
        
        builder.Services.Configure<FileStorageOptions>(
            builder.Configuration.GetSection(nameof(FileStorageOptions)));
        
        return builder;
    }
    
    /**
     * Auth
     */
    
    private static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
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
        var issuer = configuration["IDP:Issuer"] 
                     ?? throw new Exception("Issuer cannot be null");
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
                    ValidIssuer = issuer,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                };
            });
        return services;
    }
}