using Alexandria.Application.Common.Interfaces;
using Alexandria.Domain.Common.ValueObjects.Name;
using Alexandria.Domain.UserAggregate;
using ErrorOr;
using MediatR;

namespace Alexandria.Application.Users.Commands;

public record CreateUserCommand(Guid Id, string FirstName, string LastName, string? MiddleNames = null)
    : IRequest<ErrorOr<CreateUserResult>>;

public record CreateUserResult(Guid UserId);

public class CreateUserHandler : IRequestHandler<CreateUserCommand, ErrorOr<CreateUserResult>>
{
    private readonly IUserRepository _userRepository;
    public CreateUserHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<ErrorOr<CreateUserResult>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var nameResult = Name.Create(request.FirstName, request.LastName, request.MiddleNames);
        if (nameResult.IsError)
        {
            return nameResult.Errors;
        }
        
        var userResult = User.Create(request.Id, nameResult.Value);
        if (userResult.IsError)
        {
            return userResult.Errors;
        }

        var user = userResult.Value;
        var addResult = await _userRepository.AddAsync(user, cancellationToken);
        if (addResult.IsError)
        {
            return addResult.Errors;
        }
        
        var response = new CreateUserResult(user.Id);
        return response;
    }
}