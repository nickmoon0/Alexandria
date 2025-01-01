using Alexandria.Api.Tags.DTOs;
using Alexandria.Api.Users.DTOs;
using Alexandria.Application.Entries.Responses;

namespace Alexandria.Api.Entries.DTOs;

public class EntryDto
{
    public Guid? Id { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    public DocumentDto? Document { get; init; }
    public IReadOnlyList<CommentDto>? Comments { get; init; } = [];
    public IReadOnlyList<TagDto>? Tags { get; init; } = [];
    public UserDto? CreatedBy { get; init; }
    public DateTime? CreatedAtUtc { get; init; }
    public DateTime? DeletedAtUtc { get; init; }

    public static EntryDto? FromEntryResponse(EntryResponse? entry) =>
        entry == null 
            ? null 
            : new EntryDto
            {
                Id = entry.Id,
                Name = entry.Name,
                Description = entry.Description,
                Document = DocumentDto.FromDocumentResponse(entry.Document),
                Comments = (IReadOnlyList<CommentDto>?)entry.Comments?.Select(CommentDto.FromCommentResponse).ToList(),
                Tags = (IReadOnlyList<TagDto>?)entry.Tags?.Select(TagDto.FromTagResponse).ToList(),
                CreatedBy = UserDto.FromUserResponse(entry.CreatedBy),
                CreatedAtUtc = entry.CreatedAtUtc,
                DeletedAtUtc = entry.DeletedAtUtc
            };
}