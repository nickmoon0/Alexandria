using Alexandria.Application.Common.Roles;
using Alexandria.Application.Entries.Commands;
using Alexandria.CoreApi.Common;
using Alexandria.CoreApi.Common.Extensions;
using Alexandria.CoreApi.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Alexandria.CoreApi.Entries;

public abstract class AddComment : EndpointBase, IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("{entryId:guid}/comments", Handle)
        .WithSummary("Creates a comment on the given entry")
        .WithName(nameof(AddComment))
        .RequireAuthorization<User>();

    private record Request(string Content);

    private record Response(Guid CommentId);
    private static async Task<IResult> Handle(
        [FromRoute] Guid entryId,
        [FromBody] Request request,
        [FromServices] IMediator mediator,
        [FromServices] ILogger<AddComment> logger)
    {
        if (UserId == null)
        {
            logger.LogError("UserId is missing");
            return Results.Unauthorized();
        }
        
        var command = new CreateCommentRequest(entryId, (Guid)UserId, request.Content);
        var result = await mediator.Send(command);

        if (result.IsError)
        {
            return result.ToHttpResponse();
        }
        
        return Results.StatusCode(StatusCodes.Status201Created);
    }
}