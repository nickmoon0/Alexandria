using Alexandria.Api.Common.Interfaces;
using Alexandria.Api.Domain;
using Alexandria.Api.Features.People;
using Alexandria.Api.Features.People.Endpoints;

namespace Alexandria.Api;

public static class ConfigureEndpoints
{
    public static void AddEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/api")
            .WithOpenApi();
        
        endpoints.MapPersonEndpoints();
    }
    
    private static void MapPersonEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/person")
            .WithTags(nameof(Person));

        endpoints
            .MapEndpoint<CreatePerson>()
            .MapEndpoint<DeletePerson>()
            .MapEndpoint<GetPerson>();
    }
    
    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app)
        where TEndpoint : IEndpoint
    {
        TEndpoint.Map(app);
        return app;
    }
}