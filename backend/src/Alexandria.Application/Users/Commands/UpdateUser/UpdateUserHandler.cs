using Alexandria.Application.Common.Interfaces;
using Alexandria.Domain.Common.ValueObjects.Name;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alexandria.Application.Users.Commands.UpdateUser;

public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, ErrorOr<Updated>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UpdateUserHandler> _logger;
    
    public UpdateUserHandler(ILogger<UpdateUserHandler> logger, IUserRepository userRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
    }
    
    public async Task<ErrorOr<Updated>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Update user request for ID: {ID}", request.Id);
        
        var userResult = await _userRepository.FindByIdAsync(request.Id, cancellationToken);
        if (userResult.IsError)
        {
            _logger.LogError("Could not retrieve user with ID: {ID}", request.Id);
            return userResult.Errors;
        }
        
        var user = userResult.Value;

        var nameResult = Name.Create(request.FirstName, request.LastName);
        if (nameResult.IsError)
        {
            _logger.LogError("Could not create Name ValueObject for request: {Request}", request);
            return nameResult.Errors;
        }
        var name = nameResult.Value;

        user.UpdateName(name);
        var updateResult = await _userRepository.UpdateAsync(cancellationToken);
        if (updateResult.IsError)
        {
            _logger.LogError("Failed to update user with ID: {ID}", request.Id);
            return updateResult.Errors;
        }

        _logger.LogInformation("User updated with ID: {ID}", request.Id);
        return Result.Updated;
    }
}