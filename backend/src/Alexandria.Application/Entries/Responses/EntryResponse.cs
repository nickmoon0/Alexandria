using Alexandria.Application.Common.Responses;
using Alexandria.Domain.Common.Entities.Tag;
using Alexandria.Domain.EntryAggregate;

namespace Alexandria.Application.Entries.Responses;

public class EntryResponse
{
    public Guid? Id { get; init; }
    public string? Name { get; private init; }
    public string? Description { get; private init; }
    public DocumentResponse? Document { get; private init; }
    public IReadOnlyList<CommentResponse>? Comments { get; private init; }
    public IReadOnlyList<TagResponse>? Tags { get; set; }
    public Guid? CreatedById { get; private init; }
    public DateTime? CreatedAtUtc { get; private init; }
    public DateTime? DeletedAtUtc { get; private init; }

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