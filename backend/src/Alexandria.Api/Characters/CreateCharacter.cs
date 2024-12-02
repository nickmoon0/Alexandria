using Alexandria.Api.Common;
using Alexandria.Api.Common.Interfaces;
using Alexandria.Api.Common.Roles;
using Alexandria.Application.Characters.Commands;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Alexandria.Api.Characters;

public abstract class CreateCharacter : EndpointBase, IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("", Handle)
        .WithSummary("Creates a new character")
        .WithName(nameof(CreateCharacter))
        .RequireAuthorization(nameof(Admin));
    
    private record Request(
        string FirstName,
        string LastName,
        string? MiddlesNames,
        string? Description);

    private record Response(Guid Id);

    private static async Task<Results<CreatedAtRoute<Response>, BadRequest>> Handle(
        [FromBody] Request request,
        [FromServices] IMediator mediator)
    {
        var command = new CreateCharacterCommand(
            request.FirstName,
            request.LastName,
            request.MiddlesNames,
            request.Description,
            null,
            (Guid)UserId!);
        
        var result = await mediator.Send(command);
        if (result.IsError)
        {
            return TypedResults.BadRequest();
        }
        
        var response = new Response(result.Value.Id);
        return TypedResults.CreatedAtRoute(response, nameof(GetCharacter), response);
    }
}