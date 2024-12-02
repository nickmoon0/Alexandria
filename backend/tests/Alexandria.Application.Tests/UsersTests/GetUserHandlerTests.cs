using Alexandria.Application.Tests.TestUtils.Repositories;
using Alexandria.Application.Users.Queries;
using Alexandria.Domain.Tests.TestUtils.Factories;
using Alexandria.Domain.UserAggregate;
using ErrorOr;
using FluentAssertions;

namespace Alexandria.Application.Tests.UsersTests;

public class GetUserHandlerTests
{
    private readonly TestUserRepository _testUserRepository;
    private readonly GetUserHandler _handler;

    public GetUserHandlerTests()
    {
        var name1 = NameFactory.CreateName(firstName: "FirstName1", lastName: "LastName1").Value;
        var name2 = NameFactory.CreateName(firstName: "FirstName2", lastName: "LastName2").Value;

        var user1 = UserFactory.CreateUser(name1).Value;
        var user2 = UserFactory.CreateUser(name2).Value;
        
        // Arrange: Set up test data
        var testUsers = new List<User>
        {
            user1,
            user2
        };

        _testUserRepository = new TestUserRepository(testUsers);
        _handler = new GetUserHandler(_testUserRepository);
    }
    
    [Fact]
    public async Task GetUser_WhenUserExists_ShouldReturnUser()
    {
        // Arrange
        var existingUser = _testUserRepository.Users.First().Value;
        var query = new GetUserQuery(existingUser.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
        result.Value.Should().Be(result.Value);
    }

    [Fact]
    public async Task GetUser_WhenUserDoesNotExist_ShouldReturnNotFoundError()
    {
        // Arrange
        var nonExistentUserId = Guid.NewGuid();
        var query = new GetUserQuery(nonExistentUserId);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        result.Errors.Should().Contain(Error.NotFound());
    }
}