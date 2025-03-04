using Alexandria.CoreApi.Common.Extensions;
using Alexandria.Application.Characters.Queries;
using Alexandria.Application.Common.Roles;
using Alexandria.CoreApi.Characters.DTOs;
using Alexandria.CoreApi.Common;
using Alexandria.CoreApi.Common.Interfaces;
using Alexandria.CoreApi.Users.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Alexandria.CoreApi.Characters;

public abstract class GetCharacter : EndpointBase, IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/{id:guid}", Handle)
        .WithSummary("Retrieves a character by ID")
        .WithName(nameof(GetCharacter))
        .RequireAuthorization<User>();
    
    private static async Task<IResult> Handle(
        [FromRoute] Guid id,
        [FromServices] IMediator mediator)
    {
        var query = new GetCharacterQuery(id);
        var result = await mediator.Send(query);

        if (result.IsError)
        {
            return result.ToHttpResponse();
        }
        
        var character = result.Value.Character;
        var response = CharacterDto.FromCharacterResponse(character);
        
        return Results.Ok(response);
    }
}