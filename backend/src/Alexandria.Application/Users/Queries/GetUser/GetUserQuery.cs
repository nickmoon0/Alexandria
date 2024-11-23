using ErrorOr;
using MediatR;

namespace Alexandria.Application.Users.Queries.GetUser;

public record GetUserQuery : IRequest<ErrorOr<GetUserResult>>
{
    public Guid UserId { get; set; }
}