using Alexandria.CoreApi.Common.Middleware;

namespace Alexandria.CoreApi;

public static class ConfigureApp
{
    public static WebApplication AddApp(this WebApplication app)
    {
        app.AddEndpoints();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseCors(ConfigureServices.LocalHost5173CorsPolicy);
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseAntiforgery();
        
        app.UseMiddleware<EndpointInitializationMiddleware>();

        app.UseHttpsRedirection();
        
        return app;
    }
}