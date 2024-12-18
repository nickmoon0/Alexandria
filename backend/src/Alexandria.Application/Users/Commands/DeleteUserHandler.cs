using Alexandria.Application.Common.Interfaces;
using Alexandria.Domain.Common.Interfaces;
using Alexandria.Domain.UserAggregate;
using ErrorOr;
using MediatR;

namespace Alexandria.Application.Users.Commands;

public record DeleteUserCommand(Guid Id) : IRequest<ErrorOr<Success>>;

public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, ErrorOr<Success>>
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IAppDbContext _context;
    
    public DeleteUserHandler(IDateTimeProvider dateTimeProvider, IAppDbContext context)
    {
        _dateTimeProvider = dateTimeProvider;
        _context = context;
    }
    
    public async Task<ErrorOr<Success>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FindAsync([request.Id], cancellationToken);
        if (user == null) return UserErrors.NotFound;
        
        user.Delete(_dateTimeProvider);

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success;
    }
}