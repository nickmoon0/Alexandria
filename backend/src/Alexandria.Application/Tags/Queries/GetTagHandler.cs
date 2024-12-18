using Alexandria.Application.Common.Interfaces;
using Alexandria.Application.Tags.Responses;
using Alexandria.Domain.Common.Entities.Tag;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Alexandria.Application.Tags.Queries;

public record GetTagQuery(Guid? Id = null) : IRequest<ErrorOr<GetTagResponse>>;
public record GetTagResponse(IEnumerable<TagResponse> Tags);

public class GetTagHandler : IRequestHandler<GetTagQuery, ErrorOr<GetTagResponse>>
{
    private readonly ILogger<GetTagHandler> _logger;
    private readonly IAppDbContext _context;

    public GetTagHandler(ILogger<GetTagHandler> logger, IAppDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<ErrorOr<GetTagResponse>> Handle(GetTagQuery request, CancellationToken cancellationToken)
    {
        var tags = new List<Tag>();

        if (request.Id == null)
        {
            var tagsResult = await _context.Tags.ToListAsync(cancellationToken);
            tags.AddRange(tagsResult);
        }
        else
        {
            var tag = await _context.Tags.FindAsync([(Guid)request.Id], cancellationToken);
            if (tag == null)
            {
                _logger.LogError("Tag not found with ID {ID}", request.Id);
                return TagErrors.TagNotFound;
            }

            tags.Add(tag);
        }
        
        var tagResponses = tags.Select(TagResponse.FromTag);
        return new GetTagResponse(tagResponses);
    }
}