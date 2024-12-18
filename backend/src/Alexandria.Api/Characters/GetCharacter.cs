using Alexandria.Api.Characters.DTOs;
using Alexandria.Api.Common;
using Alexandria.Api.Common.Extensions;
using Alexandria.Api.Common.Interfaces;
using Alexandria.Api.Common.Roles;
using Alexandria.Api.Users.DTOs;
using Alexandria.Application.Characters.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Alexandria.Api.Characters;

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

        UserDto? userDto = null;

        if (character.User != null)
        {
            userDto = new UserDto
            {
                Id = character.User.Id,
                FirstName = character.User.Name.FirstName,
                LastName = character.User.Name.LastName,
            };
        }

        var createdByDto = new UserDto
        {
            Id = character.CreatedBy!.Id,
            FirstName = character.CreatedBy.Name.FirstName,
            LastName = character.CreatedBy.Name.LastName,
        };
        
        var response = new CharacterDto
        {
            Id = character.Id,
            FirstName = character.Name!.FirstName,
            LastName = character.Name.LastName,
            MiddleNames = character.Name.MiddleNames,
            Description = character.Description,
            User = userDto,
            CreatedBy = createdByDto,
            CreatedOnUtc = character.CreatedAtUtc
        };
        
        return Results.Ok(response);
    }
}