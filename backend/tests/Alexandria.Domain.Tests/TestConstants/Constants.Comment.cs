using Alexandria.Domain.Common.Interfaces;
using Alexandria.Domain.Tests.TestUtils.Services;

namespace Alexandria.Domain.Tests.TestConstants;

public static partial class Constants
{
    public static class Comment
    {
        public static readonly Guid DocumentId = Guid.NewGuid();
        public static readonly string Content = "This is a comment.";
        public static readonly Guid CreatedById = Guid.NewGuid();
        public static readonly IDateTimeProvider DateTimeProvider = new TestDateTimeProvider();
    }
}