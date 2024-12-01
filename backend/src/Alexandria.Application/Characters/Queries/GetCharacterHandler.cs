using Alexandria.Application.Common.Interfaces;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alexandria.Application.Characters.Queries;

public record GetCharacterQuery(Guid Id) : IRequest<ErrorOr<GetCharacterResult>>;
public record GetCharacterResult(
    Guid Id,
    string FirstName,
    string LastName,
    string? MiddlesNames,
    string? Description,
    Guid? UserId,
    Guid CreatedById,
    DateTime CreatedAtUtc);

public class GetCharacterHandler : IRequestHandler<GetCharacterQuery, ErrorOr<GetCharacterResult>>
{
    private readonly ICharacterRepository _characterRepository;
    private readonly ILogger<GetCharacterHandler> _logger;

    public GetCharacterHandler(ICharacterRepository characterRepository, ILogger<GetCharacterHandler> logger)
    {
        _characterRepository = characterRepository;
        _logger = logger;
    }

    public async Task<ErrorOr<GetCharacterResult>> Handle(GetCharacterQuery request, CancellationToken cancellationToken)
    {
        var characterResult = await _characterRepository.FindByIdAsync(request.Id, cancellationToken);
        if (characterResult.IsError)
        {
            _logger.LogInformation("Character not found with ID: {ID}", request.Id);
            return characterResult.Errors;
        }
        
        var character = characterResult.Value;
        var result = new GetCharacterResult(
            character.Id,
            character.Name.FirstName,
            character.Name.LastName,
            character.Name.MiddleNames,
            character.Description,
            character.UserId,
            character.CreatedById,
            character.CreatedAtUtc);
        
        return result;
    }
}