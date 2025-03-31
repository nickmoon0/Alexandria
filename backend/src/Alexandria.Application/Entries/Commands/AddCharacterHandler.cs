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

public record AddCharacterCommand(
    IReadOnlyList<Role> Roles,
    Guid RequestingUserId,
    Guid EntryId,
    Guid CharacterId) : IRequest<ErrorOr<Success>>;

public class AddCharacterHandler : IRequestHandler<AddCharacterCommand, ErrorOr<Success>>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<AddCharacterHandler> _logger;

    public AddCharacterHandler(IAppDbContext context, ILogger<AddCharacterHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ErrorOr<Success>> Handle(AddCharacterCommand request, CancellationToken cancellationToken)
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

        // Check that user has permission. Users can add characters to their own entries, admins can add them to anyone's
        var canAddCharacter =
            entry.CreatedById == request.RequestingUserId ||
            request.Roles.ContainsRole(new Admin());
        
        if (!canAddCharacter)
        {
            _logger.LogInformation("User '{UserId}' is not authorised to add character '{CharacterId}' to entry '{EntryId}'",
                request.RequestingUserId,
                request.CharacterId,
                request.EntryId);
            return Error.Unauthorized();
        }

        var addCharacterResult = entry.AddCharacter(character);
        if (addCharacterResult.IsError)
        {
            _logger.LogError("Failed to add character '{CharacterId}' to entry '{EntryId}'",
                request.CharacterId, request.EntryId);
            return addCharacterResult.Errors;
        }
        
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("User '{UserId}' successfully added Character '{CharacterId}' to Entry '{EntryId}'",
            request.RequestingUserId,
            request.CharacterId,
            request.EntryId);
        return Result.Success;
    }
}