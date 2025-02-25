using System.Text.Json;
using Alexandria.Application.Characters.Queries;
using Alexandria.Application.Common.Pagination;
using Alexandria.Application.Common.Roles;
using Alexandria.CoreApi.Characters.DTOs;
using Alexandria.CoreApi.Common;
using Alexandria.CoreApi.Common.Extensions;
using Alexandria.CoreApi.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Alexandria.CoreApi.Characters;

public class GetCharacters : EndpointBase, IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("", Handle)
        .WithSummary("Gets paged characters")
        .WithName(nameof(GetCharacters))
        .RequireAuthorization<User>();

    private static async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        [FromQuery] string? pageRequest = null)
    {
        var paginatedRequest = pageRequest == null
            ? new PaginatedRequest()
            : JsonSerializer.Deserialize<PaginatedRequest>(pageRequest);
        
        if (paginatedRequest == null) return Results.InternalServerError("Error parsing pageRequest");

        var query = new GetCharactersQuery(paginatedRequest);
        var result = await mediator.Send(query);
        if (result.IsError)
        {
            return result.ToHttpResponse();
        }

        var charactersPageResult = result.Value.Characters;
        
        var characterDtoList = charactersPageResult.Data
            .Select(CharacterDto.FromCharacterResponse)
            .Where(character => character != null)
            .Cast<CharacterDto>()
            .ToList();

        var pageResponse = new PaginatedResponse<CharacterDto>
        {
            Data = characterDtoList,
            Paging = charactersPageResult.Paging,
        };
        
        return Results.Ok(pageResponse);
    }
}