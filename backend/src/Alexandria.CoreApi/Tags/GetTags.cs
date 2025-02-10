using Alexandria.CoreApi.Common.Extensions;
using Alexandria.Application.Tags.Queries;
using Alexandria.CoreApi.Common;
using Alexandria.CoreApi.Common.Interfaces;
using Alexandria.CoreApi.Common.Roles;
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

    private static async Task<IResult> Handle([FromServices] IMediator mediator)
    {
        var result = await mediator.Send(new GetTagQuery());
        var tagResponses = result.Value.Tags;

        var response = tagResponses.Select(x => new TagDto
        {
            Id = x.Id,
            Name = x.Name,
        });

        return Results.Ok(response);
    }
}