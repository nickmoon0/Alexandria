using Alexandria.Application.Common.Interfaces;
using Alexandria.Domain.CharacterAggregate;
using Alexandria.Domain.Common.Interfaces;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alexandria.Application.Characters.Commands;

public record DeleteCharacterCommand(Guid Id) : IRequest<ErrorOr<Deleted>>;

public class DeleteCharacterHandler : IRequestHandler<DeleteCharacterCommand, ErrorOr<Deleted>>
{
    private readonly IAppDbContext _context;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<DeleteCharacterHandler> _logger;
    
    public DeleteCharacterHandler(
        IAppDbContext context,
        IDateTimeProvider dateTimeProvider,
        ILogger<DeleteCharacterHandler> logger)
    {
        _context = context;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }
    
    public async Task<ErrorOr<Deleted>> Handle(DeleteCharacterCommand request, CancellationToken cancellationToken)
    {
        var character = await _context.Characters.FindAsync([request.Id], cancellationToken);
        if (character == null)
        {
            _logger.LogInformation("No character found with ID: {ID}", request.Id);
            return CharacterErrors.NotFound;
        }
        
        var deleteResult = character.Delete(_dateTimeProvider);
        if (deleteResult.IsError)
        {
            _logger.LogError("Failed to delete character with ID: {ID}", character.Id);    
            return deleteResult.Errors;
        }
        
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Deleted;
    }
}