using Alexandria.Domain.Common.ValueObjects.Name;
using Alexandria.Domain.Tests.TestUtils.Factories;
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
    }
    
    [Fact]
    public void Create_WithEmptyFirstName_ShouldReturnError()
    {
        // Arrange
        const string firstName = "";
        
        // Act
        var result = NameFactory.CreateName(firstName: firstName);
    
        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(NameErrors.InvalidFirstName);
    }
    
    [Fact]
    public void Create_WithEmptyLastName_ShouldReturnError()
    {
        // Arrange
        const string lastName = "";
    
        // Act
        var result = NameFactory.CreateName(lastName: lastName);
    
        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(NameErrors.InvalidLastName);
    }
    
    [Fact]
    public void Create_WithEmptyMiddleNames_ShouldReturnName()
    {
        // Arrange
        const string middleName = "";
    
        // Act
        var result = NameFactory.CreateName(middleName: middleName);
    
        // Assert
        result.IsError.Should().BeFalse();
    }

    [Fact]
    public void Create_WithNullMiddleNames_ShouldReturnName()
    {
        const string firstName = "ValidFirst";
        const string lastName = "ValidLast";
        
        // Act
        var result = Name.Create(firstName, lastName);
    
        // Assert
        result.IsError.Should().BeFalse();
    }
    
    [Fact]
    public void Create_WithTooLongFirstName_ShouldReturnError()
    {
        // Arrange
        const string firstName = "ThisFirstNameIsWayTooLong";
    
        // Act
        var result = NameFactory.CreateName(firstName: firstName);
    
        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(NameErrors.InvalidFirstName);
    }
    
    [Fact]
    public void Create_WithTooLongLastName_ShouldReturnError()
    {
        // Arrange
        const string lastName = "ThisLastNameIsWayTooLong";
    
        // Act
        var result = NameFactory.CreateName(lastName: lastName);
    
        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(NameErrors.InvalidLastName);
    }
    
    [Fact]
    public void Create_WithTooLongMiddleNames_ShouldReturnError()
    {
        // Arrange
        const string middleName = "TheseMiddleNamesAreDefinitelyWayTooLong";
    
        // Act
        var result = NameFactory.CreateName(middleName: middleName);
    
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
        var result = NameFactory.CreateName(firstName, lastName, middleName);
    
        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(NameErrors.InvalidFirstName);
        result.Errors.Should().Contain(NameErrors.InvalidLastName);
    }
}