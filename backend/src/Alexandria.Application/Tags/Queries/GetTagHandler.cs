using Alexandria.Application.Common.Interfaces;
using Alexandria.Application.Tags.Responses;
using Alexandria.Domain.Common.Entities.Tag;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alexandria.Application.Tags.Queries;

public record GetTagQuery(Guid? Id = null) : IRequest<ErrorOr<GetTagResponse>>;
public record GetTagResponse(IEnumerable<TagResponse> Tags);

public class GetTagHandler : IRequestHandler<GetTagQuery, ErrorOr<GetTagResponse>>
{
    private readonly ILogger<GetTagHandler> _logger;
    private readonly ITagRepository _tagRepository;

    public GetTagHandler(ILogger<GetTagHandler> logger, ITagRepository tagRepository)
    {
        _logger = logger;
        _tagRepository = tagRepository;
    }

    public async Task<ErrorOr<GetTagResponse>> Handle(GetTagQuery request, CancellationToken cancellationToken)
    {
        var tags = new List<Tag>();

        if (request.Id == null)
        {
            var tagsResult = await _tagRepository.GetAllTags(cancellationToken);
            tags.AddRange(tagsResult.Value);
        }
        else
        {
            var tagResult = await _tagRepository.FindByIdAsync((Guid)request.Id, cancellationToken);
            if (tagResult.IsError)
            {
                _logger.LogError("Tag not found with ID {ID}", request.Id);
                return tagResult.Errors;
            }
            var tag = tagResult.Value;

            tags.Add(tag);
        }
        
        var tagResponses = tags.Select(TagResponse.FromTag);
        
        return new GetTagResponse(tagResponses);
    }
}