using Alexandria.Api.Common;
using Alexandria.Api.Common.Extensions;
using Alexandria.Api.Common.Interfaces;
using Alexandria.Api.Common.Roles;
using Alexandria.Api.Entries.DTOs;
using Alexandria.Api.Tags.DTOs;
using Alexandria.Api.Users.DTOs;
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
        .RequireAuthorization<User>();

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
        
        var entryResult = result.Value.Entry;

        if (entryResult.Document != null)
        {
            document = new DocumentDto
            {
                Id = entryResult.Document.Id,
                EntryId = entryResult.Document.EntryId,
                FileExtension = entryResult.Document.FileExtension,
                CreatedBy = new UserDto
                {
                    Id = (Guid)entryResult.Document.CreatedByUser!.Id!,
                    FirstName = entryResult.Document.CreatedByUser!.FirstName!,
                    LastName = entryResult.Document.CreatedByUser!.LastName!
                },
                CreatedAtUtc = entryResult.Document.CreatedAtUtc,
                DeletedAtUtc = entryResult.Document.DeletedAtUtc,
            };
        }

        var comments = entryResult.Comments?.Select(comment => new CommentDto
        {
            Id = comment.Id,
            Content = comment.Content,
            CreatedAtUtc = comment.CreatedAtUtc,
            CreatedBy = new UserDto
            {
                Id = (Guid)comment.CreatedBy!.Id!,
                FirstName = comment.CreatedBy!.FirstName!,
                LastName = comment.CreatedBy!.LastName!
            },
            DeletedAtUtc = comment.DeletedAtUtc,
        }).ToList() ?? [];


        var tags = entryResult.Tags?.Select(tag => new TagDto
        {
            Id = tag.Id,
            Name = tag.Name,
        }).ToList() ?? [];
        
        var entry = new EntryDto
        {
            Id = entryResult.Id,
            Name = entryResult.Name,
            Description = entryResult.Description,
            Document = document,
            Comments = comments,
            Tags = tags,
            CreatedBy = new UserDto
            {
                Id = (Guid)entryResult.CreatedBy!.Id!,
                FirstName = entryResult.CreatedBy!.FirstName!,
                LastName = entryResult.CreatedBy!.LastName!
            },
            CreatedAtUtc = entryResult.CreatedAtUtc,
            DeletedAtUtc = entryResult.DeletedAtUtc,
        };
        
        return Results.Ok(entry);
    }
}