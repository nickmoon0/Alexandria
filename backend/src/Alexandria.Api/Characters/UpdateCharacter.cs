using Alexandria.Api.Common;
using Alexandria.Api.Common.Extensions;
using Alexandria.Api.Common.Interfaces;
using Alexandria.Api.Common.Roles;
using Alexandria.Application.Characters.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Alexandria.Api.Characters;

public abstract class UpdateCharacter : EndpointBase, IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPatch("/{id:guid}", Handle)
        .WithSummary("Patch updates a character")
        .WithName(nameof(UpdateCharacter))
        .RequireAuthorization(nameof(Admin));

    public record Request(
        string? FirstName,
        string? LastName,
        string? MiddleNames,
        string? Description);
    
    private static async Task<IResult> Handle(
        [FromRoute] Guid id,
        [FromBody] Request request,
        [FromServices] IMediator mediator)
    {
        var command = new UpdateCharacterCommand(
            id, request.FirstName, request.LastName, request.MiddleNames, request.Description);
        var result = await mediator.Send(command);

        return result.IsError ? 
            result.ToHttpResponse() : 
            Results.Ok();
    }
}