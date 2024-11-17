using Alexandria.Domain.Common.Entities;
using Alexandria.Domain.Common.ValueObjects.Name;
using FluentAssertions;

namespace Alexandria.Domain.Tests.CommonTests.EntityTests;

public class UserTests
{
    // Don't need to test invalid name because Name objects cant be invalid
    [Fact]
    public void Create_WithValidName_ShouldReturnUser()
    {
        // Arrange
        var nameResult = Name.Create("FirstName", "LastName", "MiddleNames");
        var name = nameResult.Value;
        
        // Act
        var userResult = User.Create(name);
        
        // Assert
        userResult.IsError.Should().BeFalse();
        userResult.Value.Id.Should().NotBe(Guid.Empty);
        userResult.Value.Name.Should().Be(name);
    }
}