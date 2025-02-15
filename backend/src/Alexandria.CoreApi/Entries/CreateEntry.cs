using Alexandria.Application.Common.Roles;
using Alexandria.CoreApi.Common.Extensions;
using Alexandria.Application.Entries.Commands;
using Alexandria.CoreApi.Common;
using Alexandria.CoreApi.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Alexandria.CoreApi.Entries;

public abstract class CreateEntry : EndpointBase, IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("", Handle)
        .WithSummary("Creates a new entry")
        .WithName(nameof(CreateEntry))
        .RequireAuthorization<User>();

    private record Request(string FileName, string Name, string? Description);
    private record Response(Guid EntryId, Guid DocumentId);
    
    private static async Task<IResult> Handle(
        [FromBody] Request request,
        [FromServices] IMediator mediator)
    {
        var command = new CreateEntryCommand(
            request.Name,
            request.FileName,
            (Guid)UserId!,
            request.Description);

        var commandResult = await mediator.Send(command);

        if (commandResult.IsError)
        {
            return commandResult.ToHttpResponse();
        }
        
        var result = commandResult.Value;

        return Results.CreatedAtRoute(
            nameof(GetEntry),
            new { Id = result.EntryId },
            new Response(result.EntryId, result.DocumentId));
    }

}