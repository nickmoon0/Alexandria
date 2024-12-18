using Alexandria.Common.Tests.Interfaces;
using Alexandria.Infrastructure.Persistence.Models.Tagging;
using ErrorOr;

namespace Alexandria.Infrastructure.Tests.TestUtils.Builders;

public class TaggingBuilder : IBuilder<Tagging>
{
    private Guid _tagId = Guid.NewGuid();
    private string? _entityType;
    private Guid _entityId = Guid.NewGuid();

    public TaggingBuilder WithTagId(Guid tagId)
    {
        _tagId = tagId;
        return this;
    }

    public TaggingBuilder WithEntityType(string entityType)
    {
        _entityType = entityType;
        return this;
    }

    public TaggingBuilder WithEntityId(Guid entityId)
    {
        _entityId = entityId;
        return this;
    }
    
    public ErrorOr<Tagging> Build()
    {
        if (_entityType == null)
        {
            return Error.Validation();
        }
        
        return Tagging.Create(_tagId, _entityType, _entityId);
    }
}