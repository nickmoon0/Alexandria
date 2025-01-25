using System.Text.Json;
using Alexandria.Api.Common;
using Alexandria.Api.Common.Extensions;
using Alexandria.Api.Common.Interfaces;
using Alexandria.Api.Common.Roles;
using Alexandria.Api.Entries.DTOs;
using Alexandria.Application.Common.Pagination;
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
        [FromQuery] string? pageRequest = null,
        [FromQuery] string? options = null)
    {
        var paginatedRequest = pageRequest == null
            ? new PaginatedRequest()
            : JsonSerializer.Deserialize<PaginatedRequest>(pageRequest);

        if (paginatedRequest == null) return Results.InternalServerError("Error parsing pageRequest");
        
        var filterOptions = options?.ParseToEnumFlags<GetEntriesOptions>() ?? default;
        
        var query = new GetEntriesQuery(paginatedRequest, filterOptions);
        var result = await mediator.Send(query);
        if (result.IsError)
        {
            return result.ToHttpResponse();
        }

        var entriesPageResult = result.Value.Entries;
        
        
        var entryDtoList = entriesPageResult.Data
            .Select(EntryDto.FromEntryResponse)
            .Where(entry => entry != null)
            .Cast<EntryDto>()
            .ToList();
        
        var pageResponse = new PaginationResponse<EntryDto>
        {
            Data = entryDtoList,
            Paging = entriesPageResult.Paging,
        };
        
        return Results.Ok(pageResponse);
    }
}