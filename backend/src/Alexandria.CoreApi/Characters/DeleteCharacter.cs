using Alexandria.CoreApi.Common.Extensions;
using Alexandria.Application.Characters.Commands;
using Alexandria.CoreApi.Common;
using Alexandria.CoreApi.Common.Interfaces;
using Alexandria.CoreApi.Common.Roles;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Alexandria.CoreApi.Characters;

public abstract class DeleteCharacter : EndpointBase, IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapDelete("/{id:guid}", Handle)
        .WithSummary("Deletes a character with the given ID")
        .WithName(nameof(DeleteCharacter))
        .RequireAuthorization<Admin>();

    private static async Task<IResult> Handle(
        [FromRoute] Guid id,
        [FromServices] IMediator mediator)
    {
        var command = new DeleteCharacterCommand(id);
        var result = await mediator.Send(command);
        
        return result.IsError ? 
            result.ToHttpResponse() : 
            Results.Ok();
    }
}