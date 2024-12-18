using Alexandria.Domain.Tests.TestUtils.Factories;
using Alexandria.Domain.Tests.TestUtils.Services;
using Alexandria.Domain.UserAggregate;
using ErrorOr;
using FluentAssertions;

namespace Alexandria.Domain.Tests.UserAggregateTests;

public class UserTests
{
    // Don't need to test invalid name because Name objects cant be invalid
    [Fact]
    public void Create_WithValidName_ShouldReturnUser()
    {
        // Arrange
        var name = NameFactory.CreateName().Value;
        
        // Act
        var userResult = User.Create(Guid.NewGuid(), name);
        
        // Assert
        userResult.IsError.Should().BeFalse();
    }
    
    [Fact]
    public void Delete_WhenUserNotDeleted_ShouldSetDeletedAtUtcAndReturnDeleted()
    {
        // Arrange
        var utcNow = DateTime.UtcNow;
        var dateTimeProviderMock = new TestDateTimeProvider(utcNow);

        var user = UserFactory.CreateUser().Value;

        // Act
        var result = user.Delete(dateTimeProviderMock);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(Result.Deleted);
        user.DeletedAtUtc.Should().Be(utcNow);
    }

    [Fact]
    public void Delete_WhenUserAlreadyDeleted_ShouldReturnError()
    {
        // Arrange
        var utcNow = DateTime.UtcNow;
        var dateTimeProviderMock = new TestDateTimeProvider(utcNow);

        var user = UserFactory.CreateUser().Value;

        user.Delete(dateTimeProviderMock); // First deletion

        // Act
        var result = user.Delete(dateTimeProviderMock);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(UserErrors.AlreadyDeleted);
    }

    [Fact]
    public void RecoverDeleted_WhenUserIsDeleted_ShouldClearDeletedAtUtcAndReturnSuccess()
    {
        // Arrange
        var utcNow = DateTime.UtcNow;
        var dateTimeProviderMock = new TestDateTimeProvider(utcNow);

        var user = UserFactory.CreateUser().Value;

        user.Delete(dateTimeProviderMock); // Delete the user

        // Act
        var result = user.RecoverDeleted();

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().Be(Result.Success);
        user.DeletedAtUtc.Should().BeNull();
    }

    [Fact]
    public void RecoverDeleted_WhenUserIsNotDeleted_ShouldReturnError()
    {
        // Arrange
        var user = UserFactory.CreateUser().Value;

        // Act
        var result = user.RecoverDeleted();

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(UserErrors.NotDeleted);
    }
}