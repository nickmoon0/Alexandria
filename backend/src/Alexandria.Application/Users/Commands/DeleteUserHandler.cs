using Alexandria.Application.Common.Interfaces;
using Alexandria.Domain.Common.Interfaces;
using ErrorOr;
using MediatR;

namespace Alexandria.Application.Users.Commands;

public record DeleteUserCommand(Guid Id) : IRequest<ErrorOr<Success>>;

public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, ErrorOr<Success>>
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IUserRepository _userRepository;
    
    public DeleteUserHandler(IDateTimeProvider dateTimeProvider, IUserRepository userRepository)
    {
        _dateTimeProvider = dateTimeProvider;
        _userRepository = userRepository;
    }
    
    public async Task<ErrorOr<Success>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var userResult = await _userRepository.FindByIdAsync(request.Id, cancellationToken);
        if (userResult.IsError) return userResult.Errors;
        
        var user = userResult.Value;

        user.Delete(_dateTimeProvider);
        var updateResult = await _userRepository.UpdateAsync(cancellationToken);
        if (updateResult.IsError) return updateResult.Errors;
        
        return Result.Success;
    }
}