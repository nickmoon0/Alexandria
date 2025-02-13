using Alexandria.Application;
using Alexandria.Infrastructure;
using Microsoft.AspNetCore.Http.Features;

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
        builder.WebHost.ConfigureKestrel(options =>
        {
            options.Limits.MaxRequestBodySize = 15L * 1024 * 1024 * 1024; // 15GB
            options.Limits.MaxResponseBufferSize = null;
        });
        builder.Services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = 15L * 1024 * 1024 * 1024; // 15GB
        });
        builder.Services.AddAntiforgery();

        builder.Services.AddApplication();
        builder.Services.AddInfrastructure(builder.Configuration);
        
        return builder;
    }
}