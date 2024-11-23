using Alexandria.Application.Common.Interfaces;
using ErrorOr;
using MediatR;

namespace Alexandria.Application.Users.Queries.GetUser;

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
        var response = new GetUserResult
        {
            Id = user.Id,
            FirstName = user.Name.FirstName,
            LastName = user.Name.LastName,
            MiddleNames = user.Name.MiddleNames
        };
        
        return response;
    }
}