using ErrorOr;
using MediatR;

namespace Alexandria.Application.Users.Commands.DeleteUser;

public record DeleteUserCommand(Guid Id) : IRequest<ErrorOr<Success>>;