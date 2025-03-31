using Alexandria.Application.Common;
using Alexandria.Application.Common.Interfaces;
using Alexandria.Application.Common.Roles;
using Alexandria.Domain.CharacterAggregate;
using Alexandria.Domain.EntryAggregate.Errors;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Alexandria.Application.Entries.Commands;

public record RemoveCharacterCommand(
    IReadOnlyList<Role> Roles,
    Guid RequestingUserId,
    Guid EntryId,
    Guid CharacterId) : IRequest<ErrorOr<Success>>;

public class RemoveCharacterHandler : IRequestHandler<RemoveCharacterCommand, ErrorOr<Success>>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<AddCharacterHandler> _logger;

    public RemoveCharacterHandler(IAppDbContext context, ILogger<AddCharacterHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ErrorOr<Success>> Handle(RemoveCharacterCommand request, CancellationToken cancellationToken)
    {
        var entry = await _context.Entries
            .Include(entry => entry.Characters)
            .SingleOrDefaultAsync(entry => entry.Id == request.EntryId, cancellationToken);
        var character = await _context.Characters.FindAsync([request.CharacterId], cancellationToken);
        
        if (entry == null)
        {
            _logger.LogInformation("Entry with id {EntryId} not found", request.EntryId);
            return EntryErrors.NotFound;
        }

        if (character == null)
        {
            _logger.LogInformation("Character with id {CharacterId} not found", request.CharacterId);
            return CharacterErrors.NotFound;
        }

        // Check that user has permission. Users can remove characters from their own entries, admins can remove them from anyone's
        var canRemoveCharacter =
            entry.CreatedById == request.RequestingUserId ||
            request.Roles.ContainsRole(new Admin());
        
        if (!canRemoveCharacter)
        {
            _logger.LogInformation("User '{UserId}' is not authorised to remove character '{CharacterId}' from entry '{EntryId}'",
                request.RequestingUserId,
                request.CharacterId,
                request.EntryId);
            return Error.Unauthorized();
        }
        
        var removeCharacterResult = entry.RemoveCharacter(character);

        if (removeCharacterResult.IsError)
        {
            _logger.LogError("Failed to remove character '{CharacterId}' from entry '{EntryId}'",
                request.CharacterId, request.EntryId);
            return removeCharacterResult.Errors;
        }
        
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("User '{UserId}' successfully removed Character '{CharacterId}' from Entry '{EntryId}'",
            request.RequestingUserId,
            request.CharacterId,
            request.EntryId);
        return Result.Success;
    }
}