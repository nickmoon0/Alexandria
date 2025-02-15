using Alexandria.Application.Common.Constants;
using Alexandria.Application.Documents.Queries;
using Alexandria.FileApi.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Alexandria.FileApi.Documents;

public abstract class DownloadDocument : EndpointBase, IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("", Handle)
        .WithSummary("Download specified document")
        .WithName(nameof(DownloadDocument));

    private static async Task<IResult> Handle(
        [FromServices] IMediator mediator)
    {
        if (!TokenPermissions.Contains(FilePermissions.Read))
        {
            return Results.Unauthorized();
        }
        var query = new GetDocumentFileStreamQuery(DocumentId);
        
        var queryResult = await mediator.Send(query);
        if (queryResult.IsError)
        {
            return Results.InternalServerError();
        }
        
        var documentStream = queryResult.Value.DocumentFileStream;
        var fileName = queryResult.Value.FileName;
        var contentType = queryResult.Value.ContentType;

        return documentStream == null ? 
            Results.InternalServerError() : 
            Results.File(documentStream, contentType, fileName, enableRangeProcessing: true);
    }
}