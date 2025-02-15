using Alexandria.Application.Common.Roles;
using Alexandria.CoreApi.Common.Extensions;
using Alexandria.CoreApi.Tags.DTOs;
using Alexandria.CoreApi.Users.DTOs;
using Alexandria.Application.Entries.Queries;
using Alexandria.CoreApi.Common;
using Alexandria.CoreApi.Common.Interfaces;
using Alexandria.CoreApi.Entries.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Alexandria.CoreApi.Entries;

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