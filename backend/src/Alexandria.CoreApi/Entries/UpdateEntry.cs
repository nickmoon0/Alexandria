using Alexandria.Application.Common.Roles;
using Alexandria.Application.Entries.Commands;
using Alexandria.CoreApi.Common;
using Alexandria.CoreApi.Common.Extensions;
using Alexandria.CoreApi.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Alexandria.CoreApi.Entries;

public class UpdateEntry : EndpointBase, IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPatch("{entryId:guid}", Handle)
        .WithSummary("Patch updates an entry record")
        .WithName(nameof(UpdateEntry))
        .RequireAuthorization<User>();

    private record Request(string? Name, string? Description);
    
    private static async Task<IResult> Handle(
        [FromRoute] Guid entryId,
        [FromBody] Request request,
        [FromServices] IMediator mediator)
    {
        if (UserId == null)
        {
            return Results.Unauthorized();
        }
        
        var command = new UpdateEntryCommand(
            entryId,
            (Guid)UserId,
            request.Name,
            request.Description,
            Roles[0]);
        
        var result = await mediator.Send(command);

        return result.IsError 
            ? result.ToHttpResponse() 
            : Results.Ok();
    }
}