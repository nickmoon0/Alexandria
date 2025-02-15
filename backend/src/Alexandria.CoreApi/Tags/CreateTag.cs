using Alexandria.Application.Common.Roles;
using Alexandria.CoreApi.Common.Extensions;
using Alexandria.Application.Tags.Commands;
using Alexandria.CoreApi.Common;
using Alexandria.CoreApi.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Alexandria.CoreApi.Tags;

public abstract class CreateTag : EndpointBase, IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("", HandleAsync)
        .WithSummary("Creates a tag with the specified name")
        .WithName(nameof(CreateTag))
        .RequireAuthorization<User>();

    private record Request(string Name);

    private record Response(Guid Id);
    
    private static async Task<IResult> HandleAsync(
        [FromBody] Request request,
        [FromServices] IMediator mediator)
    {
        var command = new CreateTagCommand(request.Name);
        var result = await mediator.Send(command);

        if (result.IsError)
        {
            return result.ToHttpResponse();
        }
        
        var response = new Response(result.Value.Id);
        
        return result.IsError ? 
            result.ToHttpResponse() : 
            Results.CreatedAtRoute(nameof(GetTag), response, response);
    }
}