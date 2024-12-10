using Alexandria.Api.Common.Middleware;

namespace Alexandria.Api;

public static class ConfigureApp
{
    public static WebApplication AddApp(this WebApplication app)
    {
        app.AddEndpoints();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseAntiforgery();
        
        app.UseMiddleware<EndpointInitializationMiddleware>();

        app.UseHttpsRedirection();
        
        return app;
    }
}