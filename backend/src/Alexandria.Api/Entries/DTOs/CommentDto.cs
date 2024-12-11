namespace Alexandria.Api.Entries.DTOs;

public class CommentDto
{
    public Guid? Id { get; init; }
    public string? Content { get; init; }
    public Guid? CreatedById { get; init; }
    public DateTime? CreatedAtUtc { get; init; }
    public DateTime? DeletedAtUtc { get; init; }
}