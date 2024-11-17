using Alexandria.Domain.Common;
using ErrorOr;
using FluentAssertions;

namespace Alexandria.Domain.Tests.CommonTests;

public class TaggableAggregateRootTests
{
    private class TestTaggableAggregateRoot : TaggableAggregateRoot { }
 
        [Fact]
    public void AddTag_ShouldAddTag_WhenValid()
    {
        // Arrange
        var taggable = new TestTaggableAggregateRoot();
        var tagId = Guid.NewGuid();

        // Act
        var result = taggable.AddTag(tagId);

        // Assert
        result.IsError.Should().BeFalse();
        taggable.Tags.Should().Contain(tagId);
    }

    [Fact]
    public void AddTag_ShouldReturnValidationError_WhenTagIdIsEmpty()
    {
        // Arrange
        var taggable = new TestTaggableAggregateRoot();

        // Act
        var result = taggable.AddTag(Guid.Empty);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(Error.Validation());
    }

    [Fact]
    public void AddTag_ShouldReturnConflictError_WhenTagAlreadyExists()
    {
        // Arrange
        var taggable = new TestTaggableAggregateRoot();
        var tagId = Guid.NewGuid();
        taggable.AddTag(tagId);

        // Act
        var result = taggable.AddTag(tagId);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(Error.Conflict());
    }

    [Fact]
    public void RemoveTag_ShouldRemoveTag_WhenValid()
    {
        // Arrange
        var taggable = new TestTaggableAggregateRoot();
        var tagId = Guid.NewGuid();
        taggable.AddTag(tagId);

        // Act
        var result = taggable.RemoveTag(tagId);

        // Assert
        result.IsError.Should().BeFalse();
        taggable.Tags.Should().NotContain(tagId);
    }

    [Fact]
    public void RemoveTag_ShouldReturnValidationError_WhenTagIdIsEmpty()
    {
        // Arrange
        var taggable = new TestTaggableAggregateRoot();

        // Act
        var result = taggable.RemoveTag(Guid.Empty);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(Error.Validation());
    }

    [Fact]
    public void RemoveTag_ShouldReturnNotFoundError_WhenTagDoesNotExist()
    {
        // Arrange
        var taggable = new TestTaggableAggregateRoot();
        var tagId = Guid.NewGuid();

        // Act
        var result = taggable.RemoveTag(tagId);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(Error.NotFound());
    }
}