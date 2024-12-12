using Alexandria.Api.Users.DTOs;

namespace Alexandria.Api.Entries.DTOs;

public class CommentDto
{
    public Guid? Id { get; init; }
    public string? Content { get; init; }
    public UserDto? CreatedBy { get; init; }
    public DateTime? CreatedAtUtc { get; init; }
    public DateTime? DeletedAtUtc { get; init; }
}