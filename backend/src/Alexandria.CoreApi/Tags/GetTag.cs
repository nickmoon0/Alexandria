using Alexandria.CoreApi.Common.Extensions;
using Alexandria.Application.Tags.Queries;
using Alexandria.CoreApi.Common;
using Alexandria.CoreApi.Common.Interfaces;
using Alexandria.CoreApi.Common.Roles;
using Alexandria.CoreApi.Tags.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Alexandria.CoreApi.Tags;

public abstract class GetTag : EndpointBase, IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/{id:guid}", Handle)
        .WithSummary("Gets a tag with the given ID")
        .WithName(nameof(GetTag))
        .RequireAuthorization<User>();

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