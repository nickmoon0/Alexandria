namespace Alexandria.Api.Entries.DTOs;

public class EntryDto
{
    public Guid? Id { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    public DocumentDto? Document { get; init; }
    public IReadOnlyList<CommentDto>? Comments { get; init; }
    public Guid? CreatedById { get; init; }
    public DateTime? CreatedAtUtc { get; init; }
    public DateTime? DeletedAtUtc { get; init; }
}