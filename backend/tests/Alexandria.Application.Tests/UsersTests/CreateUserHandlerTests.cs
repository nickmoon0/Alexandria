using Alexandria.Application.Tests.TestUtils.Repositories;
using Alexandria.Application.Users.Commands;
using Alexandria.Domain.Common.ValueObjects.Name;
using FluentAssertions;

namespace Alexandria.Application.Tests.UsersTests;

public class CreateUserHandlerTests
{
    private readonly TestUserRepository _testUserRepository;
    private readonly CreateUserHandler _handler;

    public CreateUserHandlerTests()
    {
        _testUserRepository = new TestUserRepository([]);
        _handler = new CreateUserHandler(_testUserRepository);
    }

    [Fact]
    public async Task CreateUser_WithValidRequest_ShouldCreateUser()
    {
        // Arrange
        var id = Guid.NewGuid();
        var command = new CreateUserCommand(id, "John", "Doe", "Middle");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();

        // Verify the user was added to the repository
        var user = await _testUserRepository.FindByIdAsync(result.Value.UserId, CancellationToken.None);
        user.IsError.Should().BeFalse();
        user.Value.Id.Should().Be(id);
        user.Value.Name.FirstName.Should().Be(command.FirstName);
        user.Value.Name.LastName.Should().Be(command.LastName);
        user.Value.Name.MiddleNames.Should().BeEquivalentTo(command.MiddleNames);
    }

    [Fact]
    public async Task CreateUser_WithInvalidName_ShouldReturnError()
    {
        // Arrange
        var id = Guid.NewGuid();
        var command = new CreateUserCommand(
            id,
            FirstName: "", // Invalid name
            LastName: "Doe",
            MiddleNames: "Middle"
        );

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(NameErrors.InvalidFirstName);
    }
}