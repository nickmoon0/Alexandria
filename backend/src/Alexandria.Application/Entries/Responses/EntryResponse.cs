using Alexandria.Application.Tags.Responses;
using Alexandria.Application.Users.Responses;

namespace Alexandria.Application.Entries.Responses;

public class EntryResponse
{
    public Guid? Id { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    public DocumentResponse? Document { get; init; }
    public IReadOnlyList<CommentResponse>? Comments { get; init; }
    public IReadOnlyList<TagResponse>? Tags { get; set; }

    public UserResponse? CreatedBy { get; init; }
    
    public DateTime? CreatedAtUtc { get; init; }
    public DateTime? DeletedAtUtc { get; init; }
}