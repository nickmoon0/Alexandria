using Alexandria.Domain.EntryAggregate;
using Alexandria.Domain.EntryAggregate.Errors;
using Alexandria.Domain.Tests.TestUtils.Factories;
using Alexandria.Domain.Tests.TestUtils.Services;
using ErrorOr;
using FluentAssertions;

namespace Alexandria.Domain.Tests.EntryAggregateTests;

public class CommentTests
{
    [Fact]
    public void Create_WithValidData_ShouldReturnComment()
    {
        // Arrange
        var createdById = Guid.NewGuid();
        const string content = "This is a valid comment.";
        var now = DateTime.UtcNow;
        var mockDateTimeProvider = new TestDateTimeProvider(now);
        var entry = EntryFactory.CreateEntry().Value;
        
        // Act
        var result = Comment.Create(entry, content, createdById, mockDateTimeProvider);

        // Assert
        result.IsError.Should().BeFalse();
        var comment = result.Value;
        comment.Should().NotBeNull();
        comment.Entry.Should().NotBeNull();
        comment.CreatedById.Should().Be(createdById);
        comment.CreatedAtUtc.Should().Be(now);
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
        result.Errors.Should().Contain(CommentErrors.InvalidId);
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
        result.Errors.Should().Contain(CommentErrors.EmptyData);
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
        result.Errors.Should().Contain(CommentErrors.EmptyData);
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