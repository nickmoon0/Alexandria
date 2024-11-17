using Alexandria.Domain.Common.Interfaces;

namespace Alexandria.Domain.Tests.TestUtils.Services;

public class TestDateTimeProvider(DateTime? fixedDateTime = null) : IDateTimeProvider
{
    public DateTime UtcNow => fixedDateTime ?? DateTime.UtcNow;
}