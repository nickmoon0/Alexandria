using Alexandria.Domain.DocumentAggregate;
using Alexandria.Domain.Tests.TestUtils.Factories;
using Alexandria.Domain.Tests.TestUtils.Services;
using ErrorOr;
using FluentAssertions;

namespace Alexandria.Domain.Tests.DocumentAggregateTests;

public class CommentTests
{
    [Fact]
    public void Create_WithValidData_ShouldReturnComment()
    {
        // Arrange
        var documentId = Guid.NewGuid();
        var createdById = Guid.NewGuid();
        const string content = "This is a valid comment.";
        var now = DateTime.UtcNow;
        var mockDateTimeProvider = new TestDateTimeProvider(now);

        // Act
        var result = Comment.Create(documentId, content, createdById, mockDateTimeProvider);

        // Assert
        result.IsError.Should().BeFalse();
        var comment = result.Value;
        comment.Should().NotBeNull();
        comment.DocumentId.Should().Be(documentId);
        comment.CreatedById.Should().Be(createdById);
        comment.CreatedAtUtc.Should().Be(now);
    }

    [Fact]
    public void Create_WithEmptyDocumentId_ShouldReturnError()
    {
        // Arrange
        var documentId = Guid.Empty;

        // Act
        var result = CommentFactory.CreateComment(documentId: documentId);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(DocumentErrors.InvalidUserId);
    }

    [Fact]
    public void Create_WithEmptyCreatedById_ShouldReturnError()
    {
        // Arrange
        var createdById = Guid.Empty;

        // Act
        var result = CommentFactory.CreateComment(createdById: createdById);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(DocumentErrors.InvalidUserId);
    }

    [Fact]
    public void Create_WithEmptyContent_ShouldReturnError()
    {
        // Arrange
        const string content = "   "; // Empty after trimming

        // Act
        var result = CommentFactory.CreateComment(content: content);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(DocumentErrors.EmptyData);
    }

    [Fact]
    public void ModifyContent_WithValidContent_ShouldReturnUpdated()
    {
        // Arrange
        const string initialContent = "This is the initial content.";
        const string newContent = "This is the updated content.";

        var comment = CommentFactory.CreateComment(content: initialContent).Value;

        // Act
        var result = comment.ModifyContent(newContent);

        // Assert
        result.IsError.Should().BeFalse();
    }

    [Fact]
    public void ModifyContent_WithEmptyContent_ShouldReturnError()
    {
        // Arrange
        const string initialContent = "This is the initial content.";
        const string newContent = "   ";
        
        var comment = CommentFactory.CreateComment(content: initialContent).Value;
        
        // Act
        var result = comment.ModifyContent(newContent);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(DocumentErrors.EmptyData);
    }
    
    [Fact]
    public void Delete_NotAlreadyDeletedComment_ShouldReturnDeleted()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var mockDateTimeProvider = new TestDateTimeProvider(now);

        var comment = CommentFactory.CreateComment(dateTimeProvider: mockDateTimeProvider).Value;

        // Act
        var result = comment.Delete(mockDateTimeProvider);

        // Assert
        result.IsError.Should().BeFalse();
        comment.DeletedAtUtc.Should().Be(now);
    }

    [Fact]
    public void Delete_WhenAlreadyDeleted_ShouldReturnError()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var mockDateTimeProvider = new TestDateTimeProvider(now);

        var comment = CommentFactory.CreateComment(dateTimeProvider: mockDateTimeProvider).Value;

        // Act
        comment.Delete(mockDateTimeProvider); // Initial delete
        var result = comment.Delete(mockDateTimeProvider); // Attempt to delete again

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(Error.Failure());
    }

    [Fact]
    public void RecoverDeleted_WhenDeleted_ShouldReturnSuccess()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var mockDateTimeProvider = new TestDateTimeProvider(now);

        var comment = CommentFactory.CreateComment(dateTimeProvider: mockDateTimeProvider).Value;
        comment.Delete(mockDateTimeProvider); // Mark as deleted

        // Act
        var result = comment.RecoverDeleted();

        // Assert
        result.IsError.Should().BeFalse();
        comment.DeletedAtUtc.Should().BeNull();
    }

    [Fact]
    public void RecoverDeleted_WhenNotDeleted_ShouldReturnError()
    {
        // Arrange
        var mockDateTimeProvider = new TestDateTimeProvider(DateTime.UtcNow);
        var comment = CommentFactory.CreateComment(dateTimeProvider: mockDateTimeProvider).Value;

        // Act
        var result = comment.RecoverDeleted();

        // Assert
        result.IsError.Should().BeTrue();
        comment.DeletedAtUtc.Should().BeNull();
    }
}