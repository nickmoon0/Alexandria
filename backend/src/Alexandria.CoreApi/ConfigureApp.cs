using Alexandria.CoreApi.Common.Middleware;

namespace Alexandria.CoreApi;

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

        app.UseAuthentication();
        app.UseAuthorization();
        
        app.UseMiddleware<EndpointInitializationMiddleware>();
        app.UseHttpsRedirection();
        
        app.AddEndpoints();
        return app;
    }
}