using Alexandria.Application.Common.Constants;
using Alexandria.Application.Common.Roles;
using Alexandria.Application.Documents.Queries;
using Alexandria.CoreApi.Common;
using Alexandria.CoreApi.Common.Extensions;
using Alexandria.CoreApi.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Alexandria.CoreApi.Documents;

public abstract class GetDocumentToken : EndpointBase, IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/{documentId:guid}/token/{filePermission:alpha}", Handle)
        .WithSummary("Returns a token to upload/download files from the FileApi")
        .WithName(nameof(GetDocumentToken))
        .RequireAuthorization<User>();

    private record Response(string Token);
    
    private static async Task<IResult> Handle(
        [FromRoute] Guid documentId,
        [FromRoute] string filePermission,
        [FromQuery] int? expiryMinutes,
        [FromServices] IMediator mediator)
    {
        if (!Enum.TryParse<FilePermissions>(filePermission, true, out var filePermissionEnum))
        {
            return Results.BadRequest();
        }

        var query = expiryMinutes != null ? 
            new GetDocumentTokenRequest(documentId, [filePermissionEnum], (int)expiryMinutes) : 
            new GetDocumentTokenRequest(documentId, [filePermissionEnum]);
        
        var result = await mediator.Send(query);

        return result.IsError ? result.ToHttpResponse() : Results.Ok(new Response(result.Value.Token));
    }
}