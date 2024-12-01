using Alexandria.Domain.Common.Interfaces;

namespace Alexandria.Infrastructure.Services;

public class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}