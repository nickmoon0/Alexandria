using Alexandria.Application.Entries.Responses;
using Alexandria.CoreApi.Characters.DTOs;
using Alexandria.CoreApi.Tags.DTOs;
using Alexandria.CoreApi.Users.DTOs;

namespace Alexandria.CoreApi.Entries.DTOs;

public class EntryDto
{
    public Guid? Id { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    public DocumentDto? Document { get; init; }
    public IReadOnlyList<CharacterDto>? Characters { get; init; }
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
                Characters = entry.Characters?.Select(CharacterDto.FromCharacterResponse).ToList(),
                Comments = (IReadOnlyList<CommentDto>?)entry.Comments?.Select(CommentDto.FromCommentResponse).ToList(),
                Tags = (IReadOnlyList<TagDto>?)entry.Tags?.Select(TagDto.FromTagResponse).ToList(),
                CreatedBy = UserDto.FromUserResponse(entry.CreatedBy),
                CreatedAtUtc = entry.CreatedAtUtc,
                DeletedAtUtc = entry.DeletedAtUtc
            };
}