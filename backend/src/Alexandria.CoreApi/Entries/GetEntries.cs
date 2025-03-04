using System.Text.Json;
using Alexandria.CoreApi.Common.Extensions;
using Alexandria.Application.Common.Pagination;
using Alexandria.Application.Common.Roles;
using Alexandria.Application.Entries.Queries;
using Alexandria.CoreApi.Common;
using Alexandria.CoreApi.Common.Interfaces;
using Alexandria.CoreApi.Entries.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Alexandria.CoreApi.Entries;

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
        [FromQuery] string? options = null,
        [FromQuery] Guid? tagId = null)
    {
        var paginatedRequest = pageRequest == null
            ? new PaginatedRequest()
            : JsonSerializer.Deserialize<PaginatedRequest>(pageRequest);

        if (paginatedRequest == null) return Results.InternalServerError("Error parsing pageRequest");
        
        var filterOptions = options?.ParseToEnumFlags<GetEntriesOptions>() ?? default;
        
        var query = new GetEntriesQuery(paginatedRequest, filterOptions, tagId);
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
        
        var pageResponse = new PaginatedResponse<EntryDto>
        {
            Data = entryDtoList,
            Paging = entriesPageResult.Paging,
        };
        
        return Results.Ok(pageResponse);
    }
}