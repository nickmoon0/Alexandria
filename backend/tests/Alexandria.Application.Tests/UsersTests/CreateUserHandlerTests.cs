using Alexandria.Application.Common.Interfaces;
using Alexandria.Application.Tests.TestUtils.Repositories;
using Alexandria.Application.Users.Commands;
using Alexandria.Domain.Common.ValueObjects.Name;
using Alexandria.Infrastructure.Persistence;
using Alexandria.Infrastructure.Tests.TestUtils.Builders;
using FluentAssertions;

namespace Alexandria.Application.Tests.UsersTests;

public class CreateUserHandlerTests
{
    private readonly IAppDbContext _context;
    private readonly TestUserRepository _testUserRepository;
    private readonly CreateUserHandler _handler;

    public CreateUserHandlerTests()
    {
        _context = new DbContextBuilder<AppDbContext>().Build().Value;
        _testUserRepository = new TestUserRepository([]);
        _handler = new CreateUserHandler(_context);
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
        var user = await _context.Users.FindAsync([result.Value.UserId]);
        user.Should().NotBeNull();
        user!.Id.Should().Be(id);
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