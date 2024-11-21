using Alexandria.Domain.Common.Interfaces;
using Alexandria.Domain.EntryAggregate;
using Alexandria.Domain.Tests.TestConstants;
using ErrorOr;

namespace Alexandria.Domain.Tests.TestUtils.Factories;

public static class CommentFactory
{
    public static ErrorOr<Comment> CreateComment(
        Entry? entry = null,
        string? content = null,
        Guid? createdById = null,
        IDateTimeProvider? dateTimeProvider = null)
    {
        return Comment.Create(
            entry ?? EntryFactory.CreateEntry().Value,
            content ?? Constants.Comment.Content,
            createdById ?? Constants.Comment.CreatedById,
            dateTimeProvider ?? Constants.Comment.DateTimeProvider);
    }
}