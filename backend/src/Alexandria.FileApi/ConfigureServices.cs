using Alexandria.Application;
using Alexandria.Infrastructure;
using Alexandria.Infrastructure.Common.Options;

namespace Alexandria.FileApi;

public static class ConfigureServices
{
    public static WebApplicationBuilder AddServices(this WebApplicationBuilder builder)
    {
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