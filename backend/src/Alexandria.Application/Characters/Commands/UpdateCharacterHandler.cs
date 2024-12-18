using Alexandria.Application.Common.Interfaces;
using Alexandria.Domain.CharacterAggregate;
using Alexandria.Domain.Common.ValueObjects.Name;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alexandria.Application.Characters.Commands;

public record UpdateCharacterCommand(
    Guid Id,
    string? FirstName,
    string? LastName,
    string? MiddleNames,
    string? Description) : IRequest<ErrorOr<Updated>>;

public class UpdateCharacterHandler : IRequestHandler<UpdateCharacterCommand, ErrorOr<Updated>>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<UpdateCharacterHandler> _logger;
    
    public UpdateCharacterHandler(IAppDbContext context, ILogger<UpdateCharacterHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ErrorOr<Updated>> Handle(UpdateCharacterCommand request, CancellationToken cancellationToken)
    {
        var character = await _context.Characters.FindAsync([request.Id], cancellationToken);
        if (character == null)
        {
            _logger.LogInformation("Character not found with ID: {ID}", request.Id);
            return CharacterErrors.NotFound;
        }
        
        if (request.FirstName != null || request.LastName != null || request.MiddleNames != null)
        {
            var nameResult = Name.Create(
                request.FirstName ?? character.Name.FirstName,
                request.LastName ?? character.Name.LastName,
                request.MiddleNames ?? character.Name.MiddleNames
            );
            if (nameResult.IsError)
            {
                _logger.LogInformation(
                    "Failed to create name with following values: {FirstName}, {LastName}, {MiddleNames}", 
                    request.FirstName, request.LastName, request.MiddleNames);
                return nameResult.Errors;
            }
            
            character.SetName(nameResult.Value);
        }

        if (request.Description != null)
        {
            var descriptionUpdateResult = character.SetDescription(request.Description);
            if (descriptionUpdateResult.IsError)
            {
                _logger.LogInformation("Failed to update description on character with ID: {ID}", request.Id);
                return descriptionUpdateResult.Errors;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        return Result.Updated;
    }
}