using Alexandria.Domain.Common.ValueObjects.Name;
using FluentAssertions;

namespace Alexandria.Domain.Tests.CommonTests.ValueObjectTests;

public class NameTests
{
    [Fact]
    public void Create_WithValidName_ShouldReturnName()
    {
        // Arrange
        const string firstName = "Library";
        const string lastName = "of";
        const string middleName = "Alexandria";
        
        // Act
        var result = Name.Create(firstName, lastName, middleName);
        
        // Assert
        result.IsError.Should().BeFalse();
        result.Value.FirstName.Should().Be(firstName);
        result.Value.LastName.Should().Be(lastName);
        result.Value.MiddleNames.Should().Be(middleName);
    }
    
    [Fact]
    public void Create_WithEmptyFirstName_ShouldReturnError()
    {
        // Arrange
        const string firstName = "";
        const string lastName = "ValidLast";
        const string middleName = "ValidMiddle";
    
        // Act
        var result = Name.Create(firstName, lastName, middleName);
    
        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(NameErrors.InvalidFirstName);
    }
    
    [Fact]
    public void Create_WithEmptyLastName_ShouldReturnError()
    {
        // Arrange
        const string firstName = "ValidFirst";
        const string lastName = "";
        const string middleName = "ValidMiddle";
    
        // Act
        var result = Name.Create(firstName, lastName, middleName);
    
        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(NameErrors.InvalidLastName);
    }
    
    [Fact]
    public void Create_WithEmptyMiddleNames_ShouldReturnName()
    {
        // Arrange
        const string firstName = "ValidFirst";
        const string lastName = "ValidLast";
        const string middleName = "";
    
        // Act
        var result = Name.Create(firstName, lastName, middleName);
    
        // Assert
        result.IsError.Should().BeFalse();
        result.Value.FirstName.Should().Be(firstName);
        result.Value.LastName.Should().Be(lastName);
        result.Value.MiddleNames.Should().Be(null); // Empty middle name gets set to null
    }

    [Fact]
    public void Create_WithNullMiddleNames_ShouldReturnName()
    {
        const string firstName = "ValidFirst";
        const string lastName = "ValidLast";
        const string? middleName = null;
        
        // Act
        var result = Name.Create(firstName, lastName, middleName);
    
        // Assert
        result.IsError.Should().BeFalse();
        result.Value.FirstName.Should().Be(firstName);
        result.Value.LastName.Should().Be(lastName);
        result.Value.MiddleNames.Should().Be(middleName);
    }
    
    [Fact]
    public void Create_WithTooLongFirstName_ShouldReturnError()
    {
        // Arrange
        const string firstName = "ThisFirstNameIsWayTooLong";
        const string lastName = "ValidLast";
        const string middleName = "ValidMiddle";
    
        // Act
        var result = Name.Create(firstName, lastName, middleName);
    
        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(NameErrors.InvalidFirstName);
    }
    
    [Fact]
    public void Create_WithTooLongLastName_ShouldReturnError()
    {
        // Arrange
        const string firstName = "ValidFirst";
        const string lastName = "ThisLastNameIsWayTooLong";
        const string middleName = "ValidMiddle";
    
        // Act
        var result = Name.Create(firstName, lastName, middleName);
    
        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(NameErrors.InvalidLastName);
    }
    
    [Fact]
    public void Create_WithTooLongMiddleNames_ShouldReturnError()
    {
        // Arrange
        const string firstName = "ValidFirst";
        const string lastName = "ValidLast";
        const string middleName = "TheseMiddleNamesAreDefinitelyWayTooLong";
    
        // Act
        var result = Name.Create(firstName, lastName, middleName);
    
        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(NameErrors.InvalidMiddleNames);
    }
    
    [Fact]
    public void Create_WithAllEmptyFields_ShouldReturn2Errors()
    {
        // Arrange
        const string firstName = "";
        const string lastName = "";
        const string middleName = "";
    
        // Act
        var result = Name.Create(firstName, lastName, middleName);
    
        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(NameErrors.InvalidFirstName);
        result.Errors.Should().Contain(NameErrors.InvalidLastName);
    }
}