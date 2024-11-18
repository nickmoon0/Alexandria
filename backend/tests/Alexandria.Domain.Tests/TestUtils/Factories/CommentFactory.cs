using Alexandria.Domain.Common.Interfaces;
using Alexandria.Domain.DocumentAggregate;
using Alexandria.Domain.Tests.TestConstants;
using ErrorOr;

namespace Alexandria.Domain.Tests.TestUtils.Factories;

public static class CommentFactory
{
    public static ErrorOr<Comment> CreateComment(
        Guid? documentId = null,
        string? content = null,
        Guid? createdById = null,
        IDateTimeProvider? dateTimeProvider = null)
    {
        return Comment.Create(
            documentId ?? Constants.Comment.DocumentId,
            content ?? Constants.Comment.Content,
            createdById ?? Constants.Comment.CreatedById,
            dateTimeProvider ?? Constants.Comment.DateTimeProvider);
    }
}