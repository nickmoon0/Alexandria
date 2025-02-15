using Alexandria.Application.Common.Roles;
using Alexandria.Application.Entries.Commands;
using Alexandria.CoreApi.Common;
using Alexandria.CoreApi.Common.Extensions;
using Alexandria.CoreApi.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Alexandria.CoreApi.Entries;

public abstract class DeleteEntry : EndpointBase, IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapDelete("{entryId:guid}", Handle)
        .WithSummary("Deletes an entry")
        .WithName(nameof(DeleteEntry))
        .RequireAuthorization<User>();

    private static async Task<IResult> Handle(
        [FromRoute] Guid entryId,
        [FromServices] ILogger<DeleteEntry> logger,
        [FromServices] IMediator mediator)
    {
        if (UserId == null)
        {
            logger.LogError("UserId is null when deleting entry with ID {ID}", entryId);
            return Results.Unauthorized();
        }
        var role = Roles[0]; // Assume only one role for now, subject to change later?
        var command = new DeleteEntryCommand(role, entryId, (Guid)UserId);

        var result = await mediator.Send(command);

        return result.IsError ? 
            result.ToHttpResponse() : 
            Results.Ok();
    }
}