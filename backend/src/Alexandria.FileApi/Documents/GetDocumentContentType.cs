using Alexandria.Application.Documents.Queries;
using Alexandria.FileApi.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Alexandria.FileApi.Documents;

public abstract class GetDocumentContentType : EndpointBase, IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapMethods("", [ HttpMethods.Head ], Handle)
        .WithSummary("Get content type of specified document")
        .WithName(nameof(GetDocumentContentType));

    private static async Task<IResult> Handle(
        [FromServices] IMediator mediator)
    {
        var query = new GetDocumentFileStreamQuery(DocumentId, GetDocumentFileStreamOptions.HeadersOnly);
        var queryResult = await mediator.Send(query);
        
        if (queryResult.IsError)
        {
            return Results.NotFound();
        }
        
        var contentType = queryResult.Value.ContentType;
        
        return Results.Text(string.Empty, contentType);
    }
}