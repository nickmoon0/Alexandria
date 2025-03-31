using Alexandria.Application.Common.Roles;
using Alexandria.Application.Entries.Commands;
using Alexandria.CoreApi.Common;
using Alexandria.CoreApi.Common.Extensions;
using Alexandria.CoreApi.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Alexandria.CoreApi.Entries;

public abstract class AddCharacterToEntry : EndpointBase, IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("{entryId:guid}/character/{characterId:guid}", Handle)
        .WithSummary("Associates a given character with specified entry")
        .WithName(nameof(AddCharacterToEntry))
        .RequireAuthorization<User>();

    private static async Task<IResult> Handle(
        [FromRoute] Guid entryId,
        [FromRoute] Guid characterId,
        [FromServices] IMediator mediator)
    {
        if (UserId == null) return Results.Unauthorized();
        
        var command = new AddCharacterCommand(Roles.ToList(), (Guid)UserId, entryId, characterId);
        var result = await mediator.Send(command);
        
        return result.IsError ? result.ToHttpResponse() : Results.Ok();
    }
}