using Alexandria.Application.Common.Interfaces;
using Alexandria.Application.Tags.Responses;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Alexandria.Application.Tags.Queries;

public record GetTagsQuery(string? SearchString = null, int MaxCount = 10) : IRequest<ErrorOr<GetTagsResponse>>;
public record GetTagsResponse(IReadOnlyList<TagResponse> Tags);

public class GetTagsHandler : IRequestHandler<GetTagsQuery, ErrorOr<GetTagsResponse>>
{
    private readonly IAppDbContext _context;
    private readonly ILogger<GetTagsHandler> _logger;

    public GetTagsHandler(IAppDbContext context, ILogger<GetTagsHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ErrorOr<GetTagsResponse>> Handle(GetTagsQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Tags.AsQueryable();
        var searchString = request.SearchString?.Trim();
        if (!string.IsNullOrEmpty(request.SearchString))
        {
            _logger.LogInformation("Retrieving tags that contain phrase: {SearchString}", searchString);
            query = query.Where(t => t.Name != null && t.Name.Contains(request.SearchString));
        }
        
        var tags = await query
            .OrderBy(tag => tag.Name)
            .Take(request.MaxCount)
            .ToListAsync(cancellationToken);

        var tagResponses = tags.Select(tag => new TagResponse
        {
            Id = tag.Id,
            Name = tag.Name
        }).ToList();

        return new GetTagsResponse(tagResponses);
    }
}