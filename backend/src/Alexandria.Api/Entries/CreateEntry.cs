using Alexandria.Api.Common;
using Alexandria.Api.Common.Extensions;
using Alexandria.Api.Common.Interfaces;
using Alexandria.Api.Common.Roles;
using Alexandria.Application.Entries.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Alexandria.Api.Entries;

public abstract class CreateEntry : EndpointBase, IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapPost("", Handle)
        .WithSummary("Creates a new entry")
        .WithName(nameof(CreateEntry))
        .RequireAuthorization<User>()
        .DisableAntiforgery();

    private record Request(IFormFile File, string Name, string? Description);

    private record Response(Guid EntryId, Guid DocumentId);
    private static async Task<IResult> Handle(
        [FromForm] Request request,
        [FromServices] IMediator mediator)
    {
        var command = new CreateEntryCommand(
            request.Name,
            request.File.FileName,
            (Guid)UserId!,
            request.Description);

        var commandResult = await mediator.Send(command);

        if (commandResult.IsError)
        {
            return commandResult.ToHttpResponse();
        }
        
        var result = commandResult.Value;

        // Copy file to permanent location
        await using (var stream = new FileStream(result.PermanentFilePath, FileMode.Create))
        {
            await request.File.CopyToAsync(stream);
        }

        return Results.CreatedAtRoute(nameof(GetEntry), new { Id = result.EntryId }, new Response(result.EntryId, result.DocumentId));
    }

}