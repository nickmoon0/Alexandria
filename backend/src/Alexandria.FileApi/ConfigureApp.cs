using Alexandria.FileApi.Common.Extensions;

namespace Alexandria.FileApi;

public static class ConfigureApp
{
    public static WebApplication AddApp(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseCors(ConfigureServices.LocalHost5173CorsPolicy);
        }

        app.UseTokenAuthentication();
        app.UseHttpsRedirection();
        
        app.AddEndpoints();
        
        return app;
    }
}