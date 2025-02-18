using Alexandria.Domain.Common.Interfaces;
using ErrorOr;

namespace Alexandria.Infrastructure.Persistence.Models.EntryCharacter;

public class EntryCharacter : ISoftDeletable
{
    public Guid EntryCharacterId { get; set; }
    public Guid EntryId { get; set; }
    public Guid CharacterId { get; set; }
    
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