using Alexandria.Application.Entries.Responses;
using Alexandria.CoreApi.Users.DTOs;

namespace Alexandria.CoreApi.Entries.DTOs;

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

    public static DocumentDto? FromDocumentResponse(DocumentResponse? documentResponse) =>
        documentResponse == null
            ? null
            : new DocumentDto
            {
                Id = documentResponse.Id,
                EntryId = documentResponse.EntryId,
                FileExtension = documentResponse.FileExtension,
                CreatedBy = UserDto.FromUserResponse(documentResponse.CreatedByUser),
                CreatedAtUtc = documentResponse.CreatedAtUtc,
                DeletedAtUtc = documentResponse.DeletedAtUtc
            };
}