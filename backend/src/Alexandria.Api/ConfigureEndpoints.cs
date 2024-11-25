using Alexandria.Api.Common.Interfaces;
using Alexandria.Api.Users;
using Alexandria.Domain.UserAggregate;

namespace Alexandria.Api;

public static class ConfigureEndpoints
{
    public static void AddEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/api")
            .RequireAuthorization()
            .WithOpenApi();
        
        endpoints.MapUserEndpoints();
    }

    private static void MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/user")
            .WithTags(nameof(User));

        endpoints
            .MapEndpoint<GetUser>()
            .MapEndpoint<CreateUser>();
    }

    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app)
        where TEndpoint : IEndpoint
    {
        TEndpoint.Map(app);
        return app;
    }
}