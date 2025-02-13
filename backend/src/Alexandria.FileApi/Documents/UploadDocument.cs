using Alexandria.Application.Common.Constants;
using Alexandria.Application.Documents.Queries;
using Alexandria.CoreApi.Common.Extensions;
using Alexandria.FileApi.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Alexandria.FileApi.Documents;

public abstract class UploadDocument : EndpointBase, IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("", Handle)
        .WithSummary("Uploads a document to an entry/document record")
        .WithName("UploadDocument")
        .DisableAntiforgery();

    private record Request(IFormFile File);
    
    private static async Task<IResult> Handle(
        [FromForm] Request request,
        [FromServices] IMediator mediator)
    {
        if (!TokenPermissions.Contains(FilePermissions.Write))
        {
            return Results.Unauthorized();
        }
        var query = new GetDocumentPathQuery(DocumentId);
        
        var queryResult = await mediator.Send(query);
        if (queryResult.IsError)
        {
            return queryResult.ToHttpResponse();
        }

        await using (var stream = new FileStream(queryResult.Value.Path, FileMode.Create))
        {
            await request.File.CopyToAsync(stream);
        }
        
        return Results.CreatedAtRoute(
            nameof(DownloadDocument),
            new { Id = queryResult.Value.DocumentId });
    }
}