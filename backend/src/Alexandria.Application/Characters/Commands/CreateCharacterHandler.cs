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
    private readonly ICharacterRepository _characterRepository;
    private readonly IUserRepository _userRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<CreateCharacterHandler> _logger;
    
    public CreateCharacterHandler(
        ICharacterRepository characterRepository,
        IUserRepository userRepository,
        IDateTimeProvider dateTimeProvider,
        ILogger<CreateCharacterHandler> logger)
    {
        _characterRepository = characterRepository;
        _userRepository = userRepository;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    public async Task<ErrorOr<CreateCharacterResult>> Handle(CreateCharacterCommand request, CancellationToken cancellationToken)
    {
        // Make sure user exists if specified
        if (request.UserId != null)
        {
            var userExists = await _userRepository.ExistsAsync((Guid)request.UserId, cancellationToken);
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
        var addResult = await _characterRepository.AddAsync(character, cancellationToken);
        if (addResult.IsError)
        {
            _logger.LogError("Failed to add character to database");
            return addResult.Errors;
        }
        
        _logger.LogInformation("Successfully created new character in database");
        
        var response = new CreateCharacterResult(character.Id);
        return response;
    }
}