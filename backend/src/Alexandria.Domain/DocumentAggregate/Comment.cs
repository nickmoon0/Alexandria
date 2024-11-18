using Alexandria.Domain.Common;
using Alexandria.Domain.Common.Interfaces;
using ErrorOr;

namespace Alexandria.Domain.DocumentAggregate;

public class Comment : Entity, IAuditable, ISoftDeletable
{
    private string? Content { get; set; }
    public Guid? DocumentId { get; set; }
    public Guid CreatedById { get; }
    public DateTime CreatedAtUtc { get; }
    public DateTime? DeletedAtUtc { get; private set; }
    
    private Comment() { }

    private Comment(
        Guid documentId,
        string content,
        Guid createdById,
        DateTime createdAtUtc,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Content = content;
        DocumentId = documentId;
        
        CreatedById = createdById;
        CreatedAtUtc = createdAtUtc;
    }
    
    public static ErrorOr<Comment> Create(
        Guid documentId, string content, Guid createdById, IDateTimeProvider dateTimeProvider)
    {
        if (documentId == Guid.Empty || createdById == Guid.Empty)
        {
            return DocumentErrors.InvalidUserId;
        }
        
        content = content.Trim();
        if (!ContentValid(content))
        {
            return DocumentErrors.EmptyData;
        }
        
        return new Comment(documentId, content, createdById, dateTimeProvider.UtcNow);
    }

    public ErrorOr<Updated> ModifyContent(string content)
    {
        if (!ContentValid(content))
        {
            return DocumentErrors.EmptyData;
        }
        
        Content = content;
        return Result.Updated;
    }
    
    public ErrorOr<Deleted> Delete(IDateTimeProvider dateTimeProvider)
    {
        if (DeletedAtUtc.HasValue)
        {
            return Error.Failure();
        }
        
        DeletedAtUtc = dateTimeProvider.UtcNow;
        return Result.Deleted;
    }

    public ErrorOr<Success> RecoverDeleted()
    {
        if (!DeletedAtUtc.HasValue)
        {
            return Error.Failure();
        }
        
        DeletedAtUtc = null;
        return Result.Success;
    }
    
    private static bool ContentValid(string content) => !string.IsNullOrEmpty(content) && content.Trim().Length >= 1;
}