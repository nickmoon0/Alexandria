using Alexandria.Application.Common.Roles;
using Alexandria.CoreApi.Common.Extensions;
using Alexandria.Application.Tags.Queries;
using Alexandria.CoreApi.Common;
using Alexandria.CoreApi.Common.Interfaces;
using Alexandria.CoreApi.Tags.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Alexandria.CoreApi.Tags;

public abstract class GetTags : EndpointBase, IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("", Handle)
        .WithSummary("Gets all tags")
        .WithName(nameof(GetTags))
        .RequireAuthorization<User>();

    private static async Task<IResult> Handle(
        [FromQuery] string? searchString,
        [FromQuery] int? maxCount,
        [FromServices] IMediator mediator)
    {
        var query = maxCount == null
            ? new GetTagsQuery(searchString)
            : new GetTagsQuery(searchString, (int)maxCount);
        
        var result = await mediator.Send(query);
        var tagResponses = result.Value.Tags;

        var response = tagResponses.Select(x => new TagDto
        {
            Id = x.Id,
            Name = x.Name,
        });

        return Results.Ok(response);
    }
}