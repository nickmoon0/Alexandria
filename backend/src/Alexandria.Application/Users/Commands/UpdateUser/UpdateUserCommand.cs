using ErrorOr;
using MediatR;

namespace Alexandria.Application.Users.Commands.UpdateUser;

public record UpdateUserCommand(Guid Id, string FirstName, string LastName, string? MiddleNames = null) 
    : IRequest<ErrorOr<Updated>>;