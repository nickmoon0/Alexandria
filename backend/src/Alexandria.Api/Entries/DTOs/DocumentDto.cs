using Alexandria.Api.Users.DTOs;

namespace Alexandria.Api.Entries.DTOs;

public class DocumentDto
{
    // Security: Don't expose file path or file name to client
    // Don't want clients to know anything about file system
    public Guid? Id { get; init; }
    public Guid? EntryId { get; init; }
    public string? FileExtension { get; init; }
    public UserDto? CreatedBy { get; init; }
    public DateTime? CreatedAtUtc { get; init; }
    public DateTime? DeletedAtUtc { get; init; }
}