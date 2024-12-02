using Alexandria.Application.Common.Interfaces;
using Alexandria.Domain.Common.Interfaces;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alexandria.Application.Characters.Commands;

public record DeleteCharacterCommand(Guid Id) : IRequest<ErrorOr<Deleted>>;

public class DeleteCharacterHandler : IRequestHandler<DeleteCharacterCommand, ErrorOr<Deleted>>
{
    private readonly ICharacterRepository _characterRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<DeleteCharacterHandler> _logger;
    
    public DeleteCharacterHandler(
        ICharacterRepository characterRepository,
        IDateTimeProvider dateTimeProvider,
        ILogger<DeleteCharacterHandler> logger)
    {
        _characterRepository = characterRepository;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }
    
    public async Task<ErrorOr<Deleted>> Handle(DeleteCharacterCommand request, CancellationToken cancellationToken)
    {
        var characterResult = await _characterRepository.FindByIdAsync(request.Id, cancellationToken);
        if (characterResult.IsError)
        {
            _logger.LogInformation("No character found with ID: {ID}", request.Id);
            return characterResult.Errors;
        }
        
        var character = characterResult.Value;
        
        var deleteResult = character.Delete(_dateTimeProvider);
        if (deleteResult.IsError)
        {
            _logger.LogError("Failed to delete character with ID: {ID}", character.Id);    
            return deleteResult.Errors;
        }
        
        var updateResult = await _characterRepository.UpdateAsync(cancellationToken);
        if (updateResult.IsError)
        {
            _logger.LogError("Failed to update database after character delete with ID: {ID}", character.Id);
            return characterResult.Errors;
        }

        return Result.Deleted;
    }
}