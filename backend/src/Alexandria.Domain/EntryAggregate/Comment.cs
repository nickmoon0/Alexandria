using Alexandria.Domain.Common;
using Alexandria.Domain.Common.Interfaces;
using Alexandria.Domain.EntryAggregate.Errors;
using ErrorOr;

namespace Alexandria.Domain.EntryAggregate;

public class Comment : Entity, IAuditable, ISoftDeletable
{
    private string? Content { get; set; }
    public Entry? Entry { get; set; }
    
    public Guid CreatedById { get; }
    public DateTime CreatedAtUtc { get; }
    public DateTime? DeletedAtUtc { get; private set; }
    
    private Comment() { }

    private Comment(
        Entry entry,
        string content,
        Guid createdById,
        DateTime createdAtUtc,
        Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Content = content;
        Entry = entry;
        
        CreatedById = createdById;
        CreatedAtUtc = createdAtUtc;
    }
    
    public static ErrorOr<Comment> Create(
        Entry entry, string content, Guid createdById, IDateTimeProvider dateTimeProvider)
    {
        content = content.Trim();
        if (!ContentValid(content))
        {
            return CommentErrors.EmptyData;
        }

        if (createdById.Equals(Guid.Empty))
        {
            return CommentErrors.InvalidId;
        }
        
        return new Comment(entry, content, createdById, dateTimeProvider.UtcNow);
    }

    public ErrorOr<Updated> ModifyContent(string content)
    {
        if (!ContentValid(content))
        {
            return CommentErrors.EmptyData;
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