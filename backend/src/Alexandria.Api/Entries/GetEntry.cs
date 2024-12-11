using Alexandria.Api.Common;
using Alexandria.Api.Common.Extensions;
using Alexandria.Api.Common.Interfaces;
using Alexandria.Api.Common.Roles;
using Alexandria.Api.Entries.DTOs;
using Alexandria.Application.Entries.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Alexandria.Api.Entries;

public abstract class GetEntry : EndpointBase, IEndpoint
{
    public static void Map(IEndpointRouteBuilder app) => app
        .MapGet("/{id:guid}", Handle)
        .WithSummary("Retrieves an entry based on the entry's ID")
        .WithName(nameof(GetEntry))
        .RequireAuthorization(nameof(User));

    private static async Task<IResult> Handle(
        [FromRoute] Guid id,
        [FromServices] IMediator mediator)
    {
        var query = new GetEntryQuery(id);
        var result = await mediator.Send(query);
        if (result.IsError)
        {
            return result.ToHttpResponse();
        }

        DocumentDto? document = null;
        IReadOnlyList<CommentDto>? comments = null;
        
        var entryResult = result.Value.Entry;

        if (entryResult.Document != null)
        {
            document = new DocumentDto
            {
                Id = entryResult.Document.Id,
                EntryId = entryResult.Document.EntryId,
                FileExtension = entryResult.Document.FileExtension,
                CreatedById = entryResult.Document.CreatedById,
                CreatedAtUtc = entryResult.Document.CreatedAtUtc,
                DeletedAtUtc = entryResult.Document.DeletedAtUtc,
            };
        }

        if (entryResult.Comments != null)
        {
            comments = entryResult.Comments.Select(comment => new CommentDto
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedAtUtc = comment.CreatedAtUtc,
                CreatedById = comment.CreatedById,
                DeletedAtUtc = comment.DeletedAtUtc,
            }).ToList();
        }

        var entry = new EntryDto
        {
            Id = entryResult.Id,
            Name = entryResult.Name,
            Description = entryResult.Description,
            Document = document,
            Comments = comments,
            CreatedById = entryResult.CreatedById,
            CreatedAtUtc = entryResult.CreatedAtUtc,
            DeletedAtUtc = entryResult.DeletedAtUtc,
        };
        
        return Results.Ok(entry);
    }
}