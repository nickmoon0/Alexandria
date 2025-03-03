using Alexandria.Application.Characters.Commands;
using Alexandria.Application.Common.Roles;
using Alexandria.CoreApi.Common;
using Alexandria.CoreApi.Common.Extensions;
using Alexandria.CoreApi.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Alexandria.CoreApi.Characters;

public abstract class RemoveTagOnCharacter : EndpointBase, IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapDelete("/{characterId:guid}/tag/{tagId:guid}", Handle)
        .WithSummary("Removes a tag from a given character")
        .WithName(nameof(RemoveTagOnCharacter))
        .RequireAuthorization<User>();

    private static async Task<IResult> Handle(
        [FromRoute] Guid characterId,
        [FromRoute] Guid tagId,
        [FromServices] IMediator mediator)
    {
        var command = new RemoveTagOnCharacterCommand(characterId, tagId);
        var result = await mediator.Send(command);
        
        return result.IsError 
            ? result.ToHttpResponse()
            : Results.Ok();
    }
}