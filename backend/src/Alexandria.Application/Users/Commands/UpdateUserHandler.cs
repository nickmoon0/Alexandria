using Alexandria.Application.Common.Interfaces;
using Alexandria.Domain.Common.ValueObjects.Name;
using Alexandria.Domain.UserAggregate;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alexandria.Application.Users.Commands;

public record UpdateUserCommand(Guid Id, string FirstName, string LastName, string? MiddleNames = null) 
    : IRequest<ErrorOr<Updated>>;

public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, ErrorOr<Updated>>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<UpdateUserHandler> _logger;
    
    public UpdateUserHandler(ILogger<UpdateUserHandler> logger, IAppDbContext context)
    {
        _logger = logger;
        _context = context;
    }
    
    public async Task<ErrorOr<Updated>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Update user request for ID: {ID}", request.Id);

        var user = await _context.Users.FindAsync([request.Id], cancellationToken);
        if (user == null)
        {
            _logger.LogError("Could not retrieve user with ID: {ID}", request.Id);
            return UserErrors.NotFound;
        }
        
        var nameResult = Name.Create(request.FirstName, request.LastName);
        if (nameResult.IsError)
        {
            _logger.LogError("Could not create Name ValueObject for request: {Request}", request);
            return nameResult.Errors;
        }
        var name = nameResult.Value;

        user.UpdateName(name);
        
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("User updated with ID: {ID}", request.Id);
        
        return Result.Updated;
    }
}