using Alexandria.Application.Characters.Responses;
using Alexandria.Application.Common;
using Alexandria.Application.Common.Interfaces;
using Alexandria.Application.Common.Pagination;
using Alexandria.Application.Tags.Responses;
using Alexandria.Application.Users.Responses;
using Alexandria.Domain.CharacterAggregate;
using Alexandria.Domain.Common.Entities.Tag;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Alexandria.Application.Characters.Queries;

public record GetCharactersQuery(PaginatedRequest PaginatedParams) : IRequest<ErrorOr<GetCharactersResponse>>;
public record GetCharactersResponse(PaginatedResponse<CharacterResponse> Characters);

public class GetCharactersHandler : IRequestHandler<GetCharactersQuery, ErrorOr<GetCharactersResponse>>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<GetCharactersHandler> _logger;
    private readonly ITaggingService _taggingService;
    
    private const int MAX_PAGE_SIZE = 100;

    public GetCharactersHandler(IAppDbContext context, ILogger<GetCharactersHandler> logger, ITaggingService taggingService)
    {
        _context = context;
        _logger = logger;
        _taggingService = taggingService;
    }

    public async Task<ErrorOr<GetCharactersResponse>> Handle(GetCharactersQuery request, CancellationToken cancellationToken)
    {
        if (request.PaginatedParams.PageSize > 100)
        {
            _logger.LogInformation(
                "User attempted to retrieve more than {PageSize} entries (exceeds maximum of {MaxPageSize}).",
                request.PaginatedParams.PageSize,
                MAX_PAGE_SIZE);
            return ApplicationErrors.BadQueryError;
        }
        
        Character? cursorCharacter;
        IQueryable<Character> query = _context.Characters;

        if (request.PaginatedParams.CursorId != null)
        {
            _logger.LogInformation("CursorId set to: {CursorId}", request.PaginatedParams.CursorId);
            
            cursorCharacter = await _context.Characters.FindAsync([request.PaginatedParams.CursorId], cancellationToken);
            if (cursorCharacter == null)
            {
                _logger.LogInformation("Character with ID \'{CursorId}\' not found", request.PaginatedParams.CursorId);
                return CharacterErrors.NotFound;
            }
            
            query = query
                .Where(character => character.CreatedAtUtc < cursorCharacter.CreatedAtUtc ||
                                    (character.CreatedAtUtc == cursorCharacter.CreatedAtUtc && character.Id < cursorCharacter.Id));
        }

        var characters = await query
            .OrderByDescending(character => character.CreatedAtUtc)
            .ThenByDescending(character => character.Id)
            .Take(request.PaginatedParams.PageSize)
            .ToListAsync(cancellationToken);
        
        _logger.LogInformation("Retrieved {CharacterCount} characters", characters.Count);

        Guid? nextCursor = null;
        if (characters.Count != 0)
        {
            var lastCharacter = characters.Last();
            var hasNextPage = await _context.Characters.AnyAsync(character =>
                character.CreatedAtUtc < lastCharacter.CreatedAtUtc ||
                (character.CreatedAtUtc == lastCharacter.CreatedAtUtc && character.Id < lastCharacter.Id),
                cancellationToken);
            if (hasNextPage)
            {
                nextCursor = lastCharacter.Id;
                _logger.LogInformation("NextCursor set to {CursorId}", nextCursor);
            }
        }

        var characterResponses = (await GetCharacterResponses(characters, cancellationToken)).ToList();
        
        var pagedResponse = new PaginatedResponse<CharacterResponse>()
        {
            Data = characterResponses,
            Paging = new PagingData { NextCursor = nextCursor }
        };
        
        return new GetCharactersResponse(pagedResponse);
    }

    private async Task<IEnumerable<CharacterResponse>> GetCharacterResponses(
        List<Character> characters,
        CancellationToken cancellationToken)
    {
        if (characters.Count == 0) return [];

        var createdByUserIds = characters
            .Select(x => x.CreatedById)
            .ToList();

        var userIds = characters
            .Where(character => character.UserId.HasValue)
            .Select(x => x.UserId!.Value)
            .ToList();

        userIds.AddRange(createdByUserIds);
        userIds = userIds.Distinct().ToList();
        
        var users = await _context.Users
            .Where(character => userIds.Contains(character.Id))
            .ToDictionaryAsync(
                user => user.Id,
                user => user,
                cancellationToken);

        var tags = await _taggingService.GetEntitiesTags(characters, cancellationToken);
        
        var characterResponses = characters
            .Select(character => new CharacterResponse
            {
                Id = character.Id,
                Name = character.Name,
                Description = character.Description,
                CreatedAtUtc = character.CreatedAtUtc,
                CreatedBy = GetUserResponse(character.CreatedById),
                User = character.UserId != null 
                    ? GetUserResponse((Guid)character.UserId) 
                    : null,
                Tags = GetTagResponses(character.Id)
            });

        return characterResponses;

        List<TagResponse>? GetTagResponses(Guid characterId) =>
            tags.TryGetValue(characterId, out var characterTags)
                ? characterTags.Select(tag => new TagResponse
                {
                    Id = tag.Id,
                    Name = tag.Name,
                }).ToList()
                : null;
        
        UserResponse? GetUserResponse(Guid userId) =>
            users.TryGetValue(userId, out var user)
                ? new UserResponse { Id = user.Id, Name = user.Name }
                : null;
    }
}