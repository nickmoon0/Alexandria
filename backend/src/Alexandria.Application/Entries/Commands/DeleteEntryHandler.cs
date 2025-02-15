using Alexandria.Application.Common.Interfaces;
using Alexandria.Application.Common.Roles;
using Alexandria.Domain.Common.Interfaces;
using Alexandria.Domain.EntryAggregate.Errors;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Alexandria.Application.Entries.Commands;

public record DeleteEntryCommand(Role Role, Guid EntryId, Guid UserId)
    : IRequest<ErrorOr<Deleted>>;
public class DeleteEntryHandler 
    : IRequestHandler<DeleteEntryCommand, ErrorOr<Deleted>>
{
    private readonly IAppDbContext _context;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<DeleteEntryHandler> _logger;
    
    public DeleteEntryHandler(IAppDbContext context, IDateTimeProvider dateTimeProvider, ILogger<DeleteEntryHandler> logger)
    {
        _context = context;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    public async Task<ErrorOr<Deleted>> Handle(DeleteEntryCommand request, CancellationToken cancellationToken)
    {
        // Note: Include all entry sub-records so that they can be deleted as well
        var entry = await _context.Entries
            .Include(entry => entry.Document)
            .SingleOrDefaultAsync(entry => entry.Id == request.EntryId, cancellationToken);
        
        if (entry == null)
        {
            _logger.LogInformation("Entry with ID {ID} not found", request.EntryId);
            return EntryErrors.NotFound;
        }

        switch (request.Role)
        {
            case User when request.UserId != entry.CreatedById:
                _logger.LogInformation("User with ID {UserID} attempted to deleted Entry with ID {EntryID}",
                    request.UserId, request.EntryId);
                return Error.Unauthorized();
            case Admin:
            case User:
                entry.Delete(_dateTimeProvider);
                await _context.SaveChangesAsync(cancellationToken);
            
                _logger.LogInformation("Entry with ID {EntryID} deleted by user {UserID}", request.EntryId, request.UserId);
                return Result.Deleted;
            default:
                _logger.LogError("Unknown role received when deleting entry.");
                return Error.Unauthorized();
        }
    }
    
}