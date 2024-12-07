using Alexandria.Domain.Common.Entities.Tag;
using FluentAssertions;

namespace Alexandria.Domain.Tests.CommonTests.EntityTests;

public class TagTests
{
    [Fact]
    public void Create_ShouldReturnTag_WhenNameIsValid()
    {
        // Arrange
        const string name = "ValidTagName";

        // Act
        var result = Tag.Create(name);

        // Assert
        result.IsError.Should().BeFalse();
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Create_ShouldReturnInvalidNameError_WhenNameIsNullOrWhitespace(string invalidName)
    {
        // Act
        var result = Tag.Create(invalidName);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(TagErrors.InvalidName);
    }

    [Fact]
    public void Create_ShouldReturnInvalidNameError_WhenNameExceedsMaxLength()
    {
        // Arrange
        var longName = new string('a', 51);

        // Act
        var result = Tag.Create(longName);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(TagErrors.InvalidName);
    }

    [Fact]
    public void Create_ShouldGenerateNewId_WhenIdIsNotProvided()
    {
        // Arrange
        const string name = "TagName";

        // Act
        var result = Tag.Create(name);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Id.Should().NotBe(Guid.Empty);
    }
}