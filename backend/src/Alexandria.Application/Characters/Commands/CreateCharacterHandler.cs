using Alexandria.Application.Common.Interfaces;
using Alexandria.Domain.CharacterAggregate;
using Alexandria.Domain.Common.Interfaces;
using Alexandria.Domain.Common.ValueObjects.Name;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alexandria.Application.Characters.Commands;

public record CreateCharacterCommand(
    string FirstName,
    string LastName,
    string? MiddlesNames,
    string? Description,
    Guid? UserId,
    Guid CreatedById) : IRequest<ErrorOr<CreateCharacterResult>>;
public record CreateCharacterResult(Guid Id);

public class CreateCharacterHandler : IRequestHandler<CreateCharacterCommand, ErrorOr<CreateCharacterResult>>
{
    private readonly IAppDbContext _context;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<CreateCharacterHandler> _logger;
    
    public CreateCharacterHandler(
        IAppDbContext context,
        IDateTimeProvider dateTimeProvider,
        ILogger<CreateCharacterHandler> logger)
    {
        _context = context;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    public async Task<ErrorOr<CreateCharacterResult>> Handle(CreateCharacterCommand request, CancellationToken cancellationToken)
    {
        // Make sure user exists if specified
        if (request.UserId != null)
        {
            var userExists = await _context.Users.FindAsync([request.UserId] , cancellationToken) != null;
            if (!userExists)
            {
                _logger.LogError("Specified user does not exist with ID: {ID}", request.UserId);
                return Error.NotFound();
            }
        }

        var nameResult = Name.Create(
            request.FirstName,
            request.LastName,
            request.MiddlesNames);
        if (nameResult.IsError)
        {
            _logger.LogError("Failed to create name");
            return nameResult.Errors;
        }
        
        var characterResult = Character.Create(
            nameResult.Value,
            request.CreatedById,
            _dateTimeProvider,
            request.Description,
            request.UserId);
        if (characterResult.IsError)
        {
            _logger.LogError("Failed to create character");
            return characterResult.Errors;
        }
        
        var character = characterResult.Value;
        await _context.Characters.AddAsync(character, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        _logger.LogInformation("Successfully created new character in database");
        
        var response = new CreateCharacterResult(character.Id);
        return response;
    }
}