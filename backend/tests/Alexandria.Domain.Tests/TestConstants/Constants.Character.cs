using Alexandria.Domain.Common.Interfaces;
using Alexandria.Domain.Tests.TestUtils.Services;

namespace Alexandria.Domain.Tests.TestConstants;

public static partial class Constants
{
    public static class Character
    {
        public static readonly Guid CreatedById = Guid.NewGuid();
        public static readonly IDateTimeProvider DateTimeProvider = new TestDateTimeProvider();
        public const string Description = "Test description";
        public static readonly Guid UserId = Guid.NewGuid();
    }
}