using Alexandria.Api.Characters.DTOs;
using Alexandria.Api.Common;
using Alexandria.Api.Common.Interfaces;
using Alexandria.Api.Common.Roles;
using Alexandria.Application.Characters.Queries;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Alexandria.Api.Characters;

public abstract class GetCharacter : EndpointBase, IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/{id:guid}", Handle)
        .WithSummary("Retrieves a character by ID")
        .WithName(nameof(GetCharacter))
        .RequireAuthorization(nameof(User));

    private record Response(CharacterDto Character);
    
    private static async Task<Results<Ok<Response>, NotFound>> Handle(
        [FromRoute] Guid id,
        [FromServices] IMediator mediator)
    {
        var query = new GetCharacterQuery(id);
        var result = await mediator.Send(query);

        if (result.IsError)
        {
            return TypedResults.NotFound();
        }
        
        var character = result.Value;
        var response = new Response(new CharacterDto
        {
            Id = character.Id,
            FirstName = character.FirstName,
            LastName = character.LastName,
            MiddleNames = character.MiddlesNames,
            Description = character.Description,
            UserId = character.UserId,
            CreatedBy = character.CreatedById,
            CreatedOnUtc = character.CreatedAtUtc
        });
        
        return TypedResults.Ok(response);
    }
}