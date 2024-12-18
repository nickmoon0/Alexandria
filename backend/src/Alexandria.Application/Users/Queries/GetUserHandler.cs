using Alexandria.Application.Common.Interfaces;
using Alexandria.Application.Users.Responses;
using Alexandria.Domain.Common.ValueObjects.Name;
using Alexandria.Domain.UserAggregate;
using ErrorOr;
using MediatR;

namespace Alexandria.Application.Users.Queries;

public record GetUserQuery(Guid UserId) : IRequest<ErrorOr<UserResponse>>;

public class GetUserHandler : IRequestHandler<GetUserQuery, ErrorOr<UserResponse>>
{
    private readonly IAppDbContext _context;

    public GetUserHandler(IAppDbContext context)
    {
        _context = context;
    }
    
    public async Task<ErrorOr<UserResponse>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FindAsync([request.UserId], cancellationToken);
        if (user == null)
        {
            return UserErrors.NotFound;
        }

        var response = new UserResponse
        {
            Id = user.Id,
            Name = user.Name,
        };
        
        return response;
    }
}