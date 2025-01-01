using Alexandria.Api.Common;
using Alexandria.Api.Common.Extensions;
using Alexandria.Api.Common.Interfaces;
using Alexandria.Api.Common.Roles;
using Alexandria.Api.Entries.DTOs;
using Alexandria.Api.Tags.DTOs;
using Alexandria.Api.Users.DTOs;
using Alexandria.Application.Entries.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Alexandria.Api.Entries;

public abstract class GetEntry : EndpointBase, IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/{id:guid}", Handle)
        .WithSummary("Retrieves an entry based on the entry's ID")
        .WithName(nameof(GetEntry))
        .RequireAuthorization<User>();
    
    private static async Task<IResult> Handle(
        [FromRoute] Guid id,
        [FromServices] IMediator mediator,
        [FromQuery] string? options)
    {
        // Parse filter options into enum flag
        var filterOptions = options?.ParseToEnumFlags<GetEntryOptions>() ?? default;
        var query = new GetEntryQuery(id, filterOptions);
        
        var result = await mediator.Send(query);
        if (result.IsError)
        {
            return result.ToHttpResponse();
        }

        var entryDto = EntryDto.FromEntryResponse(result.Value.Entry);
        return Results.Ok(entryDto);
    }
}