namespace Alexandria.Domain.Common.Interfaces;

public interface IAuditable
{
    public Guid CreatedById { get; }
    public DateTime CreatedAtUtc { get; }
}