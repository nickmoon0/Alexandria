using Alexandria.Application.Characters.Responses;
using Alexandria.Application.Common.Interfaces;
using Alexandria.Application.Users.Responses;
using Alexandria.Domain.CharacterAggregate;
using Alexandria.Domain.UserAggregate;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alexandria.Application.Characters.Queries;

public record GetCharacterQuery(Guid Id) : IRequest<ErrorOr<GetCharacterResult>>;
public record GetCharacterResult(CharacterResponse Character);

public class GetCharacterHandler : IRequestHandler<GetCharacterQuery, ErrorOr<GetCharacterResult>>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<GetCharacterHandler> _logger;

    public GetCharacterHandler(IAppDbContext context, ILogger<GetCharacterHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ErrorOr<GetCharacterResult>> Handle(GetCharacterQuery request, CancellationToken cancellationToken)
    {
        var character = await _context.Characters.FindAsync([request.Id], cancellationToken);
        if (character == null)
        {
            _logger.LogInformation("Character not found with ID: {ID}", request.Id);
            return CharacterErrors.NotFound;
        }
        
        // Get the CreatedBy user response object
        var createdByUser = await _context.Users.FindAsync([character.CreatedById], cancellationToken);
        if (createdByUser == null)
        {
            _logger.LogError("Failed to retrieve user with ID {ID}", character.CreatedById);
            return UserErrors.NotFound;
        }

        UserResponse? charUserResponse = null;
        if (character.UserId != null)
        {
            var charUser = await _context.Users.FindAsync([character.UserId], cancellationToken);
            if (charUser == null)
            {
                _logger.LogError("Failed to retrieve user with ID {ID}", character.UserId);
                return UserErrors.NotFound;
            }

            charUserResponse = new UserResponse
            {
                Id = charUser.Id,
                Name = charUser.Name,
            };
        }
        
        var createdByResponse = new UserResponse
        {
            Id = createdByUser.Id,
            Name = createdByUser.Name,
        };
        
        var response = new CharacterResponse
        {
            Id = character.Id,
            Name = character.Name,
            Description = character.Description,
            User = charUserResponse,
            CreatedBy = createdByResponse,
            CreatedAtUtc = character.CreatedAtUtc,
        };
        
        return new GetCharacterResult(response);
    }
}