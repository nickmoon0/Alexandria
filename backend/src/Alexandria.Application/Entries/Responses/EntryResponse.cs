using Alexandria.Domain.EntryAggregate;

namespace Alexandria.Application.Entries.Responses;

public class EntryResponse
{
    public Guid? Id { get; init; }
    public string? Name { get; init; }
    public string? Description { get; init; }
    public DocumentResponse? Document { get; init; }
    public IReadOnlyList<CommentResponse>? Comments { get; init; }
    public Guid? CreatedById { get; init; }
    public DateTime? CreatedAtUtc { get; init; }
    public DateTime? DeletedAtUtc { get; init; }

    public static EntryResponse FromEntry(Entry entry)
    {
        DocumentResponse? document = null;
        IReadOnlyList<CommentResponse>? comments = null;

        if (entry.Document != null)
        {
            document = DocumentResponse.FromDocument(entry.Document);
        }

        if (entry.Comments.Count > 0)
        {
            comments = entry.Comments.Select(CommentResponse.FromComment).ToList();
        }
        
        return new EntryResponse
        {
            Id = entry.Id,
            Name = entry.Name,
            Description = entry.Description,
            Document = document,
            Comments = comments,
            CreatedById = entry.CreatedById,
            CreatedAtUtc = entry.CreatedAtUtc,
            DeletedAtUtc = entry.DeletedAtUtc,
        };
    }
}