using Alexandria.Application.Comments.Queries;
using Alexandria.Application.Common.Roles;
using Alexandria.CoreApi.Common;
using Alexandria.CoreApi.Common.Extensions;
using Alexandria.CoreApi.Common.Interfaces;
using Alexandria.CoreApi.Entries.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Alexandria.CoreApi.Entries;

public abstract class GetComments : EndpointBase, IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/{entryId:guid}/comments", Handle)
        .WithSummary("Get comments from an entry")
        .WithName(nameof(GetComments))
        .RequireAuthorization<User>();
    
    private static async Task<IResult> Handle(
        [FromRoute] Guid entryId,
        [FromServices] IMediator mediator)
    {
        var query = new GetCommentsQuery(entryId);
        
        var result = await mediator.Send(query);
        return result.IsError ? 
            result.ToHttpResponse() : 
            Results.Ok(result.Value.Comments.Select(CommentDto.FromCommentResponse));
    }
}