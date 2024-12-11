using Alexandria.Domain.Common.Interfaces;
using ErrorOr;

namespace Alexandria.Infrastructure.Persistence.Models.Tagging;

public class Tagging : ISoftDeletable
{
    public Guid TaggingId { get; set; }
    public Guid TagId { get; set; }
    public required string EntityType { get; set; }
    public Guid EntityId { get; set; }

    private Tagging() { }

    public static Tagging Create(
        Guid tagId,
        string entityType,
        Guid entityId)
    {
        return new Tagging()
        {
            TagId = tagId,
            EntityType = entityType,
            EntityId = entityId
        };
    }

    public DateTime? DeletedAtUtc { get; private set; }
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
}