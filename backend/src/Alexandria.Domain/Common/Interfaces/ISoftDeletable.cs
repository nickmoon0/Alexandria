using ErrorOr;

namespace Alexandria.Domain.Common.Interfaces;

public interface ISoftDeletable
{
    public DateTime? DeletedAtUtc { get; }

    public ErrorOr<Deleted> Delete(IDateTimeProvider dateTimeProvider);
    public ErrorOr<Success> RecoverDeleted();
}