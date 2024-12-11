using Alexandria.Api.Common;
using Alexandria.Api.Common.Extensions;
using Alexandria.Api.Common.Interfaces;
using Alexandria.Api.Common.Roles;
using Alexandria.Api.Tags.DTOs;
using Alexandria.Application.Tags.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Alexandria.Api.Tags;

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