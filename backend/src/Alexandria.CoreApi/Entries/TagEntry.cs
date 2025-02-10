using Alexandria.CoreApi.Common.Extensions;
using Alexandria.Application.Entries.Commands;
using Alexandria.CoreApi.Common;
using Alexandria.CoreApi.Common.Interfaces;
using Alexandria.CoreApi.Common.Roles;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Alexandria.CoreApi.Entries;

public abstract class TagEntry : EndpointBase, IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("/{entryId:guid}/tag/{tagId:guid}", Handle)
        .WithSummary("Tags the given entry with the given tag")
        .WithName(nameof(TagEntry))
        .RequireAuthorization<User>();

    private static async Task<IResult> Handle(
        [FromRoute] Guid entryId,
        [FromRoute] Guid tagId,
        [FromServices] IMediator mediator)
    {
        var command = new TagEntryCommand(entryId, tagId);
        var result = await mediator.Send(command);

        return result.IsError ? 
            result.ToHttpResponse() : 
            Results.Ok();
    }
}