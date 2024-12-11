using Alexandria.Application.Common.Interfaces;
using Alexandria.Domain.Common.Entities.Tag;
using ErrorOr;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Alexandria.Application.Tags.Commands;

public record CreateTagCommand(string Name) : IRequest<ErrorOr<CreateTagResponse>>;
public record CreateTagResponse(Guid Id);

public class CreateTagHandler : IRequestHandler<CreateTagCommand, ErrorOr<CreateTagResponse>>
{
    private readonly ILogger<CreateTagHandler> _logger;
    private readonly ITagRepository _tagRepository;

    public CreateTagHandler(ITagRepository tagRepository, ILogger<CreateTagHandler> logger)
    {
        _tagRepository = tagRepository;
        _logger = logger;
    }

    public async Task<ErrorOr<CreateTagResponse>> Handle(CreateTagCommand request, CancellationToken cancellationToken)
    {
        var tagResult = Tag.Create(request.Name);
        if (tagResult.IsError)
        {
            _logger.LogError("Failed to create tag with name {Name}", request.Name);
            return tagResult.Errors;
        }
        var tag = tagResult.Value;
        
        var addResult = await _tagRepository.AddAsync(tag, cancellationToken);
        if (addResult.IsError)
        {
            _logger.LogError("Failed to add tag with name {Name}", request.Name);
            return addResult.Errors;
        }

        return new CreateTagResponse(tag.Id);
    }
}