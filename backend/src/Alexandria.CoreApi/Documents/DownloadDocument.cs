using Alexandria.CoreApi.Common.Extensions;
using Alexandria.Application.Documents.Queries;
using Alexandria.CoreApi.Common;
using Alexandria.CoreApi.Common.Interfaces;
using Alexandria.CoreApi.Common.Roles;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Alexandria.CoreApi.Documents;

public abstract class DownloadDocument : EndpointBase, IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapMethods("{documentId:guid}", [ HttpMethods.Get, HttpMethods.Head ], Handle)
        .WithSummary("Downloads a document with the specified Guid")
        .WithName(nameof(DownloadDocument))
        .RequireAuthorization<User>();

    private static async Task<IResult> Handle(
        [FromRoute] Guid documentId,
        [FromServices] IMediator mediator)
    {
        var query = new GetDocumentFileStreamQuery(documentId);
        
        var queryResult = await mediator.Send(query);
        if (queryResult.IsError)
        {
            return queryResult.ToHttpResponse();
        }
        
        var documentStream = queryResult.Value.DocumentFileStream;
        var fileName = queryResult.Value.FileName;
        var contentType = queryResult.Value.ContentType;
        
        return Results.File(documentStream, contentType, fileName, enableRangeProcessing: true);
    }
}