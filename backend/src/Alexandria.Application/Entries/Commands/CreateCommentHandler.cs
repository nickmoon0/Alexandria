using Alexandria.Application.Common.Interfaces;
using Alexandria.Domain.Common.Interfaces;
using Alexandria.Domain.EntryAggregate;
using Alexandria.Domain.EntryAggregate.Errors;
using Alexandria.Domain.UserAggregate;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Alexandria.Application.Entries.Commands;

public record CreateCommentRequest(Guid EntryId, Guid UserId, string Content) : IRequest<ErrorOr<CreateCommentResponse>>;

public record CreateCommentResponse(Guid CommentId);

public class CreateCommentHandler : IRequestHandler<CreateCommentRequest, ErrorOr<CreateCommentResponse>>
{
    private readonly IAppDbContext _context;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<CreateCommentHandler> _logger;
    
    public CreateCommentHandler(IAppDbContext context, IDateTimeProvider dateTimeProvider, ILogger<CreateCommentHandler> logger)
    {
        _context = context;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    public async Task<ErrorOr<CreateCommentResponse>> Handle(CreateCommentRequest request, CancellationToken cancellationToken)
    {
        var entry = await _context.Entries.FindAsync([request.EntryId], cancellationToken);
        if (entry == null)
        {
            _logger.LogInformation("Entry with ID {ID} does not exist", request.EntryId);
            return EntryErrors.NotFound;
        }
        
        var userExists = await _context.Users.AnyAsync(user => user.Id == request.UserId, cancellationToken);
        if (!userExists)
        {
            _logger.LogInformation("User with ID {ID} does not exist", request.UserId);
            return UserErrors.NotFound;
        }
        
        var commentResult = Comment.Create(entry, request.Content, request.UserId, _dateTimeProvider);

        if (commentResult.IsError)
        {
            _logger.LogInformation("Failed to create comment");
            return commentResult.Errors;
        }
        var comment = commentResult.Value;
        
        await _context.Comments.AddAsync(comment, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        
        return new CreateCommentResponse(comment.Id);
    }
}