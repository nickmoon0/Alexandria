using Alexandria.Domain.CharacterAggregate;
using Alexandria.Domain.Common.ValueObjects.Name;
using FluentAssertions;

namespace Alexandria.Domain.Tests.CharacterAggregateTests;

public class CharacterTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldReturnCharacter()
    {
        // Arrange
        var nameResult = Name.Create("First", "Last", "Middle");
        var name = nameResult.Value;
        const string description = "This is a test description";
        var userId = Guid.NewGuid();
        
        // Act
        var characterResult = Character.Create(name, description, userId);
        
        // Assert
        characterResult.IsError.Should().BeFalse();
        characterResult.Value.Id.Should().NotBe(Guid.Empty);
        characterResult.Value.Name.Should().Be(name);
        characterResult.Value.Description.Should().Be(description);
        characterResult.Value.UserId.Should().Be(userId);
    }

    [Fact]
    public void Create_WithInvalidUserId_ShouldReturnError()
    {
        // Arrange
        var nameResult = Name.Create("First", "Last", "Middle");
        var name = nameResult.Value;
        const string description = "This is a test description";
        var userId = Guid.Empty;
        
        // Act
        var characterResult = Character.Create(name, description, userId);
        
        // Assert
        characterResult.IsError.Should().BeTrue();
        characterResult.Errors.Should().Contain(CharacterErrors.InvalidUserId);
    }

    [Fact]
    public void Create_WithWhiteSpaceDescription_ShouldReturnCharacter()
    {
        // Arrange
        var nameResult = Name.Create("First", "Last", "Middle");
        var name = nameResult.Value;
        var description = "";
        description += "  "; // Two spaces
        description += "    "; // One tab
        
        // Act
        var characterResult = Character.Create(name, description);
        
        // Assert
        characterResult.IsError.Should().BeFalse();
        characterResult.Value.Id.Should().NotBe(Guid.Empty);
        characterResult.Value.Name.Should().Be(name);
        characterResult.Value.Description.Should().Be(""); // Description should get trimmed
    }
    
    [Fact]
    public void Create_WithTooLongDescription_ShouldReturnError()
    {
        // Arrange
        var nameResult = Name.Create("First", "Last", "Middle");
        var name = nameResult.Value;
        var description = "";
        for (var i = 0; i < 2001; i++) // Create a description that's too long
        {
            description += "a";
        }
        
        // Act
        var characterResult = Character.Create(name, description);
        
        // Assert
        characterResult.IsError.Should().BeTrue();
        characterResult.Errors.Should().Contain(CharacterErrors.DescriptionTooLong);
    }
}