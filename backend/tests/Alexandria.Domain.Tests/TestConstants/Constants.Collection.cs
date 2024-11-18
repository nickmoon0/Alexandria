using Alexandria.Domain.Common.Interfaces;
using Alexandria.Domain.Tests.TestUtils.Services;

namespace Alexandria.Domain.Tests.TestConstants;

public partial class Constants
{
    public static class Collection
    {
        public static readonly string Name = "TestCollectionName";
        public static readonly Guid CreatedById = Guid.NewGuid();
        public static readonly IDateTimeProvider DateTimeProvider = new TestDateTimeProvider();
    }
}