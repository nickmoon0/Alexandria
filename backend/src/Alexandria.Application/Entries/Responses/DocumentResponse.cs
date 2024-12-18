using Alexandria.Application.Users.Responses;

namespace Alexandria.Application.Entries.Responses;

public class DocumentResponse
{
    public Guid? Id { get; init; }
    public Guid? EntryId { get; init; }
    public string? Name { get; init; }
    public string? ImagePath { get; init; }
    public string? FileExtension { get; init; }
    public UserResponse? CreatedByUser { get; init; }
    public DateTime? CreatedAtUtc { get; init; }
    public DateTime? DeletedAtUtc { get; init; }
}