using Alexandria.Api.Common;
using Alexandria.Api.Common.Extensions;
using Alexandria.Api.Common.Interfaces;
using Alexandria.Application.Tags.Commands;
using Alexandria.Domain.UserAggregate;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Alexandria.Api.Tags;

public abstract class CreateTag : EndpointBase, IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("", HandleAsync)
        .WithSummary("Creates a tag with the specified name")
        .WithName(nameof(CreateTag))
        .RequireAuthorization(nameof(User));

    private record Request(string Name);

    private record Response(Guid Id);
    
    private static async Task<IResult> HandleAsync(
        [FromBody] Request request,
        [FromServices] IMediator mediator)
    {
        var command = new CreateTagCommand(request.Name);
        var result = await mediator.Send(command);

        return result.IsError ? 
            result.ToHttpResponse() : 
            Results.Ok();
    }
}