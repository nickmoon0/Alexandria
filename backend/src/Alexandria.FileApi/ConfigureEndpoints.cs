using Alexandria.Domain.EntryAggregate;
using Alexandria.FileApi.Common;
using Alexandria.FileApi.Documents;

namespace Alexandria.FileApi;

public static class ConfigureEndpoints
{
    public static void AddEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/api")
            .WithOpenApi();

        endpoints
            .MapDocumentEndpoints();
    }

    private static IEndpointRouteBuilder MapDocumentEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoints = app.MapGroup("/document")
            .WithTags(nameof(Document));

        endpoints
            .MapEndpoint<DownloadDocument>()
            .MapEndpoint<GetDocumentContentType>();
        
        return app;
    }
    
    private static IEndpointRouteBuilder MapEndpoint<TEndpoint>(this IEndpointRouteBuilder app)
        where TEndpoint : IEndpoint
    {
        TEndpoint.Map(app);
        return app;
    }
}