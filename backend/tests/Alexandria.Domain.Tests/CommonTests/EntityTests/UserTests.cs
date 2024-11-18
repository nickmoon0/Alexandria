using Alexandria.Domain.Common.Entities;
using Alexandria.Domain.Common.ValueObjects.Name;
using Alexandria.Domain.Tests.TestUtils.Factories;
using FluentAssertions;

namespace Alexandria.Domain.Tests.CommonTests.EntityTests;

public class UserTests
{
    // Don't need to test invalid name because Name objects cant be invalid
    [Fact]
    public void Create_WithValidName_ShouldReturnUser()
    {
        // Arrange
        var name = NameFactory.CreateName().Value;
        
        // Act
        var userResult = User.Create(name);
        
        // Assert
        userResult.IsError.Should().BeFalse();
    }
}