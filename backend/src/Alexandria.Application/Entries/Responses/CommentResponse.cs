using Alexandria.Domain.EntryAggregate;

namespace Alexandria.Application.Entries.Responses;

public class CommentResponse
{
    public Guid? Id { get; init; }
    public string? Content { get; init; }
    public Guid? CreatedById { get; init; }
    public DateTime? CreatedAtUtc { get; init; }
    public DateTime? DeletedAtUtc { get; init; }

    public static CommentResponse FromComment(Comment comment)
    {
        return new CommentResponse
        {
            Id = comment.Id,
            Content = comment.Content,
            CreatedById = comment.CreatedById,
            CreatedAtUtc = comment.CreatedAtUtc,
            DeletedAtUtc = comment.DeletedAtUtc,
        };
    }
}