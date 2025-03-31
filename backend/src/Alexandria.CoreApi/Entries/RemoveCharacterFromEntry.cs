using Alexandria.CoreApi.Common.Extensions;
using Alexandria.Application.Common.Roles;
using Alexandria.Application.Entries.Commands;
using Alexandria.CoreApi.Common;
using Alexandria.CoreApi.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Alexandria.CoreApi.Entries;

public abstract class RemoveCharacterFromEntry : EndpointBase, IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapDelete("{entryId:guid}/character/{characterId:guid}", Handle)
        .WithSummary("Associates a given character with specified entry")
        .WithName(nameof(RemoveCharacterFromEntry))
        .RequireAuthorization<User>();

    private static async Task<IResult> Handle(
        [FromRoute] Guid entryId,
        [FromRoute] Guid characterId,
        [FromServices] IMediator mediator)
    {
        if (UserId == null) return Results.Unauthorized();
        
        var command = new RemoveCharacterCommand(Roles.ToList(), (Guid)UserId, entryId, characterId);
        var result = await mediator.Send(command);
        
        return result.IsError ? result.ToHttpResponse() : Results.Ok();
    }
}