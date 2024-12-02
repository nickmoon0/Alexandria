using Alexandria.Api.Common;
using Alexandria.Api.Common.Interfaces;
using Alexandria.Api.Common.Roles;
using Alexandria.Application.Characters.Commands;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Alexandria.Api.Characters;

public class DeleteCharacter : EndpointBase, IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapDelete("/{id:guid}", Handle)
        .WithSummary("Deletes a character with the given ID")
        .WithName(nameof(DeleteCharacter))
        .RequireAuthorization(nameof(Admin));

    private static async Task<Results<Ok, InternalServerError>> Handle(
        [FromRoute] Guid id,
        [FromServices] IMediator mediator)
    {
        var command = new DeleteCharacterCommand(id);
        var result = await mediator.Send(command);

        return result.IsError ? 
            TypedResults.InternalServerError() : 
            TypedResults.Ok();
    }
}