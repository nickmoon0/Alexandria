using Alexandria.Application.Common.Interfaces;
using Alexandria.Application.Users.Queries;
using Alexandria.Domain.Tests.TestUtils.Factories;
using Alexandria.Domain.UserAggregate;
using Alexandria.Infrastructure.Persistence;
using Alexandria.Infrastructure.Tests.TestUtils.Builders;
using FluentAssertions;

namespace Alexandria.Application.Tests.UsersTests;

public class GetUserHandlerTests
{
    private readonly IAppDbContext _context;
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
        
        _context = new DbContextBuilder<AppDbContext>().Build().Value;
        _handler = new GetUserHandler(_context);
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
        result.Errors.Should().Contain(UserErrors.NotFound);
    }
    
    [Fact]
    public async Task GetUser_WhenUserExists_ShouldReturnUser()
    {
        // Arrange
        var user = UserFactory.CreateUser(id: Guid.NewGuid()).Value;
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
        
        var existingUser = await _context.Users.FindAsync([user.Id]);
        var query = new GetUserQuery(existingUser!.Id);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeFalse();
        result.Value.Should().NotBeNull();
    }
}