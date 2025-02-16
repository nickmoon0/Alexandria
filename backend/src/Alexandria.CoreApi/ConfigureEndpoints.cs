using Alexandria.CoreApi.Characters;
using Alexandria.CoreApi.Common.Interfaces;
using Alexandria.CoreApi.Documents;
using Alexandria.CoreApi.Entries;
using Alexandria.CoreApi.Tags;
using Alexandria.CoreApi.Users;
using Alexandria.Domain.CharacterAggregate;
using Alexandria.Domain.Common.Entities.Tag;
using Alexandria.Domain.EntryAggregate;
using Alexandria.Domain.UserAggregate;

namespace Alexandria.CoreApi;

public static class ConfigureEndpoints
{
    public static void AddEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/api")
            .RequireAuthorization()
            .WithOpenApi();
        
        endpoints
            .MapCharacterEndpoints()
            .MapDocumentEndpoints()
            .MapEntryEndpoints()
            .MapTagEndpoints()
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

    private static IEndpointRouteBuilder MapDocumentEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/document")
            .WithTags(nameof(Document));

        endpoints
            .MapEndpoint<GetDocumentToken>();

        return app;
    }
    
    private static IEndpointRouteBuilder MapEntryEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/entry")
            .WithTags(nameof(Entry));

        endpoints
            .MapEndpoint<AddComment>()
            .MapEndpoint<CreateEntry>()
            .MapEndpoint<DeleteEntry>()
            .MapEndpoint<GetEntries>()
            .MapEndpoint<GetEntry>()
            .MapEndpoint<TagEntry>();

        return app;
    }
    
    private static IEndpointRouteBuilder MapTagEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/tag")
            .WithTags(nameof(Tag));

        endpoints
            .MapEndpoint<CreateTag>()
            .MapEndpoint<GetTag>()
            .MapEndpoint<GetTags>();

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