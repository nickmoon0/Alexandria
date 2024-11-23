using ErrorOr;
using MediatR;

namespace Alexandria.Application.Users.Commands.CreateUser;

public record CreateUserCommand(string FirstName, string LastName, string? MiddleNames = null)
    : IRequest<ErrorOr<CreateUserResult>>;