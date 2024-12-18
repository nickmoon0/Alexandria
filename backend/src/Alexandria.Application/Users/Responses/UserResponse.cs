using Alexandria.Domain.Common.ValueObjects.Name;

namespace Alexandria.Application.Users.Responses;

public class UserResponse
{
    public required Guid Id { get; init; }
    public required Name Name { get; init; }
}