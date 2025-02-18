using Alexandria.Application.Common.Interfaces;
using Alexandria.Application.Entries.Responses;
using Alexandria.Application.Users.Responses;
using Alexandria.Domain.EntryAggregate.Errors;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Alexandria.Application.Comments.Queries;

public record GetCommentsQuery(Guid? EntryId = null) : IRequest<ErrorOr<GetCommentsResponse>>;
public record GetCommentsResponse(IReadOnlyList<CommentResponse> Comments);

public class GetCommentsHandler : IRequestHandler<GetCommentsQuery, ErrorOr<GetCommentsResponse>>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<GetCommentsHandler> _logger;

    public GetCommentsHandler(IAppDbContext context, ILogger<GetCommentsHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ErrorOr<GetCommentsResponse>> Handle(GetCommentsQuery request, CancellationToken cancellationToken)
    {
        var entryExists = await _context.Entries.AnyAsync(entry => entry.Id == request.EntryId, cancellationToken);
        if (!entryExists)
        {
            _logger.LogInformation("Entry with ID {ID} not found", request.EntryId);
            return EntryErrors.NotFound;
        }
        
        var comments = await _context.Comments
            .Where(x => x.Entry != null && x.Entry.Id == request.EntryId)
            .ToListAsync(cancellationToken);
        var commentAuthorIds = comments.Select(comment => comment.CreatedById).ToList();
        
        var commentAuthors = await _context.Users
            .Where(user => commentAuthorIds.Contains(user.Id))
            .Distinct()
            .ToDictionaryAsync(x => x.Id, x => x, cancellationToken);

        var commentResponses = comments.Select(comment => 
            new CommentResponse 
            {
                Id = comment.Id,
                Content = comment.Content,
                CreatedBy = GetUser(comment.CreatedById),
                CreatedAtUtc = comment.CreatedAtUtc,
                DeletedAtUtc = comment.DeletedAtUtc,
            })
            .ToList();

        return new GetCommentsResponse(commentResponses);

        // Used to safely get a userResponse object
        UserResponse? GetUser(Guid id) => 
            commentAuthors.TryGetValue(id, out var author) ? 
                new UserResponse { Id = author.Id, Name = author.Name.ShallowClone(), } : 
                null;
    }
}