using Alexandria.Application;
using Alexandria.Infrastructure;
using Alexandria.Infrastructure.Common.Options;

namespace Alexandria.FileApi;

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

        builder.AddOptions();
        
        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration);
        
        return builder;
    }
    
    private static WebApplicationBuilder AddOptions(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<RabbitMqOptions>(
            builder.Configuration.GetSection(nameof(RabbitMqOptions)));
        
        builder.Services.Configure<FileStorageOptions>(
            builder.Configuration.GetSection(nameof(FileStorageOptions)));
        
        return builder;
    }
}