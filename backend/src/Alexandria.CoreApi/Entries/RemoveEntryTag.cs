using Alexandria.Application.Common.Roles;
using Alexandria.Application.Entries.Commands;
using Alexandria.CoreApi.Common;
using Alexandria.CoreApi.Common.Extensions;
using Alexandria.CoreApi.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Alexandria.CoreApi.Entries;

public abstract class RemoveEntryTag : EndpointBase, IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapDelete("/{entryId:guid}/tag/{tagId:guid}", Handle)
        .WithSummary("Removes a tag from an entry")
        .WithName(nameof(RemoveEntryTag))
        .RequireAuthorization<User>();

    private static async Task<IResult> Handle(
        [FromRoute] Guid entryId,
        [FromRoute] Guid tagId,
        [FromServices] IMediator mediator)
    {
        var command = new RemoveEntryTagCommand(entryId, tagId);
        var result = await mediator.Send(command);

        return result.IsError
            ? result.ToHttpResponse()
            : Results.Ok();
    }
}