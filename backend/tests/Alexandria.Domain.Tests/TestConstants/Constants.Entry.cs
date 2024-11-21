using Alexandria.Domain.Common.Interfaces;
using Alexandria.Domain.Tests.TestUtils.Services;

namespace Alexandria.Domain.Tests.TestConstants;

public static partial class Constants
{
    public static class Entry
    {
        public static readonly string Name = "Entry";
        public static readonly Guid CreatedById = Guid.NewGuid();
        public static readonly IDateTimeProvider DateTimeProvider = new TestDateTimeProvider();
        public static readonly string Description = "Description";
    }
}