using Alexandria.Application.Common.Interfaces;
using Alexandria.Application.Common.Roles;
using Alexandria.Domain.EntryAggregate.Errors;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alexandria.Application.Entries.Commands;

public record UpdateEntryCommand(
    Guid EntryId,
    Guid RequestingUserId,
    string? Name,
    string? Description,
    Role? UserRole = null) : IRequest<ErrorOr<Updated>>;

public class UpdateEntryHandler : IRequestHandler<UpdateEntryCommand, ErrorOr<Updated>>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<UpdateEntryHandler> _logger;

    public UpdateEntryHandler(IAppDbContext context, ILogger<UpdateEntryHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ErrorOr<Updated>> Handle(UpdateEntryCommand request, CancellationToken cancellationToken)
    {
        var entry = await _context.Entries.FindAsync([request.EntryId], cancellationToken);
        if (entry is null)
        {
            _logger.LogInformation("Entry with id {EntryId} not found", request.EntryId);
            return EntryErrors.NotFound;
        }

        // Users can delete their own entries, Admins can delete anyone's entries
        if (entry.CreatedById != request.RequestingUserId && request.UserRole is not Admin)
        {
            _logger.LogInformation("User with ID {UserID} is not authorised to delete entry with ID {EntryID}",
                request.RequestingUserId, request.EntryId);
            return Error.Unauthorized();
        }
        
        _logger.LogInformation(
            "User with ID {UserID} is updating Entry with ID {EntryID}",
            request.RequestingUserId, request.EntryId);
            
        if (request.Name is not null) entry.UpdateName(request.Name);
        if (request.Description is not null) entry.UpdateDescription(request.Description);
            
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Successfully updated entry with ID {EntryID}", request.EntryId);

        return Result.Updated;

    }
}