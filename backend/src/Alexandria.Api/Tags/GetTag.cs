using Alexandria.Api.Common;
using Alexandria.Api.Common.DTOs;
using Alexandria.Api.Common.Extensions;
using Alexandria.Api.Common.Interfaces;
using Alexandria.Api.Common.Roles;
using Alexandria.Application.Tags.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Alexandria.Api.Tags;

public abstract class GetTag : EndpointBase, IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/{id:guid}", Handle)
        .WithSummary("Gets a tag with the given ID")
        .WithName(nameof(GetTag))
        .RequireAuthorization(nameof(User));

    private static async Task<IResult> Handle(
        [FromRoute] Guid id,
        [FromServices] IMediator mediator)
    {
        var query = new GetTagQuery(id);
        var result = await mediator.Send(query);

        if (result.IsError)
        {
            return result.ToHttpResponse();
        }

        var tag = result.Value.Tags.First();
        
        var tagDto = new TagDto
        {
            Id = tag.Id,
            Name = tag.Name
        };
        
        return Results.Ok(tagDto);
    }
}