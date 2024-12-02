using Alexandria.Api.Characters;
using Alexandria.Api.Common.Interfaces;
using Alexandria.Api.Users;
using Alexandria.Domain.CharacterAggregate;
using Alexandria.Domain.UserAggregate;

namespace Alexandria.Api;

public static class ConfigureEndpoints
{
    public static void AddEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/api")
            .RequireAuthorization()
            .WithOpenApi();
        
        endpoints
            .MapCharacterEndpoints()
            .MapUserEndpoints();
    }

    private static IEndpointRouteBuilder MapCharacterEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/character")
            .WithTags(nameof(Character));

        endpoints
            .MapEndpoint<CreateCharacter>()
            .MapEndpoint<DeleteCharacter>()
            .MapEndpoint<GetCharacter>()
            .MapEndpoint<UpdateCharacter>();
        
        return app;
    }
    
    private static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/user")
            .WithTags(nameof(User));

        endpoints
            .MapEndpoint<GetUser>();

        return app;
    }
    
    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app)
        where TEndpoint : IEndpoint
    {
        TEndpoint.Map(app);
        return app;
    }
}