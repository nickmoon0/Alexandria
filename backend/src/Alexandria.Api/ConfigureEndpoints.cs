using Alexandria.Api.Characters;
using Alexandria.Api.Common.Interfaces;
using Alexandria.Api.Entries;
using Alexandria.Api.Users;
using Alexandria.Domain.CharacterAggregate;
using Alexandria.Domain.EntryAggregate;
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
            .MapEntryEndpoints()
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

    private static IEndpointRouteBuilder MapEntryEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/entry")
            .WithTags(nameof(Entry));

        endpoints
            .MapEndpoint<CreateEntry>()
            .MapEndpoint<GetEntry>();

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