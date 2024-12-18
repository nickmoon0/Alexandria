using Alexandria.Domain.Common.Interfaces;
using Alexandria.Domain.Tests.TestUtils.Services;

namespace Alexandria.Domain.Tests.TestConstants;

public static partial class Constants
{
    public static class Document
    {
        public static readonly Guid EntryId = Guid.NewGuid();
        public static readonly string Name = "Test Document Name";
        public static readonly string FileExtension = ".jpg";
        public static readonly string ImagePath = "test/image/path.jpg";
        public static readonly Guid CreatedById = Guid.NewGuid();
        public static readonly IDateTimeProvider DateTimeProvider = new TestDateTimeProvider();
        public static readonly string Description = "Test Document Description";
    }
}