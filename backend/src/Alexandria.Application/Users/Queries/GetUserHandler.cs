using Alexandria.Application.Common.Interfaces;
using ErrorOr;
using MediatR;

namespace Alexandria.Application.Users.Queries;

public record GetUserQuery(Guid UserId) : IRequest<ErrorOr<GetUserResult>>;
public record GetUserResult(Guid Id, string FirstName, string LastName, string? MiddleNames);

public class GetUserHandler : IRequestHandler<GetUserQuery, ErrorOr<GetUserResult>>
{
    private readonly IUserRepository _userRepository;

    public GetUserHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<ErrorOr<GetUserResult>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var userResult = await _userRepository.FindByIdAsync(request.UserId, cancellationToken);

        if (userResult.IsError)
        {
            return userResult.Errors;
        }

        var user = userResult.Value;
        var response = new GetUserResult(
            user.Id,
            user.Name.FirstName,
            user.Name.LastName,
            user.Name.MiddleNames
        );
        
        return response;
    }
}