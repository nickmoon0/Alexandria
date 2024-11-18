using Alexandria.Domain.CharacterAggregate;
using Alexandria.Domain.Common.ValueObjects.Name;
using Alexandria.Domain.Tests.TestUtils.Factories;
using Alexandria.Domain.Tests.TestUtils.Services;
using ErrorOr;
using FluentAssertions;

namespace Alexandria.Domain.Tests.CharacterAggregateTests;

public class CharacterTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldReturnCharacter()
    {
        // Arrange
        var name = NameFactory.CreateName().Value;
        var createdById = Guid.NewGuid();
        var dateTimeProvider = new TestDateTimeProvider();
        const string description = "This is a test description";
        var userId = Guid.NewGuid();
        
        // Act
        var characterResult = Character.Create(name, createdById, dateTimeProvider, description, userId);

        // Assert
        characterResult.IsError.Should().BeFalse();
    }

    [Fact]
    public void Create_WithInvalidUserId_ShouldReturnError()
    {
        // Arrange
        var userId = Guid.Empty;
        
        // Act
        var characterResult = CharacterFactory.CreateCharacter(userId: userId);
        
        // Assert
        characterResult.IsError.Should().BeTrue();
        characterResult.Errors.Should().Contain(CharacterErrors.InvalidUserId);
    }

    [Fact]
    public void Create_WithWhiteSpaceDescription_ShouldReturnCharacter()
    {
        // Arrange
        var description = "";
        description += "  "; // Two spaces
        description += "    "; // One tab
        
        // Act
        var characterResult = CharacterFactory.CreateCharacter(description: description);
        
        // Assert
        characterResult.IsError.Should().BeFalse();
    }
    
    [Fact]
    public void Create_WithTooLongDescription_ShouldReturnError()
    {
        // Arrange
        var description = new string('a', 2001);

        // Act
        var characterResult = CharacterFactory.CreateCharacter(description: description);
        
        // Assert
        characterResult.IsError.Should().BeTrue();
        characterResult.Errors.Should().Contain(CharacterErrors.DescriptionTooLong);
    }
    
    [Fact]
    public void Delete_NotAlreadyDeletedCharacter_ShouldReturnDeleted()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var mockDateTimeProvider = new TestDateTimeProvider(now);

        var character = CharacterFactory.CreateCharacter().Value;

        // Act
        var result = character.Delete(mockDateTimeProvider);

        // Assert
        result.IsError.Should().BeFalse();
        character.DeletedAtUtc.Should().Be(now);
    }

    [Fact]
    public void Delete_WhenAlreadyDeleted_ShouldReturnError()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var mockDateTimeProvider = new TestDateTimeProvider(now);

        var character = CharacterFactory.CreateCharacter().Value;

        // Act
        character.Delete(mockDateTimeProvider); // Initial delete
        var result = character.Delete(mockDateTimeProvider); // Attempt to delete again

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

        var character = CharacterFactory.CreateCharacter().Value;
        character.Delete(mockDateTimeProvider); // Mark as deleted

        // Act
        var result = character.RecoverDeleted();

        // Assert
        result.IsError.Should().BeFalse();
        character.DeletedAtUtc.Should().BeNull();
    }

    [Fact]
    public void RecoverDeleted_WhenNotDeleted_ShouldReturnError()
    {
        // Arrange
        var character = CharacterFactory.CreateCharacter().Value;

        // Act
        var result = character.RecoverDeleted();

        // Assert
        result.IsError.Should().BeTrue();
        character.DeletedAtUtc.Should().BeNull();
    }
}