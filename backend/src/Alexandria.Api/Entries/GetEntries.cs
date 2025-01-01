using Alexandria.Api.Common;
using Alexandria.Api.Common.Extensions;
using Alexandria.Api.Common.Interfaces;
using Alexandria.Api.Common.Roles;
using Alexandria.Api.Entries.DTOs;
using Alexandria.Application.Entries.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Alexandria.Api.Entries;

public abstract class GetEntries : EndpointBase, IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("", Handle)
        .WithSummary("Retrieves all entries within paged results")
        .WithName(nameof(GetEntries))
        .RequireAuthorization<User>();

    private static async Task<IResult> Handle(
        [FromServices] IMediator mediator,
        [FromQuery] Guid? lastPagedId = null,
        [FromQuery] DateTime? lastPagedDate = null,
        [FromQuery] int count = 30,
        [FromQuery] string? options = null)
    {
        var filterOptions = options?.ParseToEnumFlags<GetEntriesOptions>() ?? default;
        
        var query = new GetEntriesQuery(lastPagedId, lastPagedDate, count, filterOptions);
        var result = await mediator.Send(query);
        if (result.IsError)
        {
            return result.ToHttpResponse();
        }

        var entryDtoList = result.Value.Entries.Select(EntryDto.FromEntryResponse);
        return Results.Ok(entryDtoList);
    }
}