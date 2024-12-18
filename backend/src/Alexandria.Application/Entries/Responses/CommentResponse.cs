using Alexandria.Application.Users.Responses;

namespace Alexandria.Application.Entries.Responses;

public class CommentResponse
{
    public Guid? Id { get; init; }
    public string? Content { get; init; }
    public UserResponse? CreatedBy { get; init; }
    public DateTime? CreatedAtUtc { get; init; }
    public DateTime? DeletedAtUtc { get; init; }
}