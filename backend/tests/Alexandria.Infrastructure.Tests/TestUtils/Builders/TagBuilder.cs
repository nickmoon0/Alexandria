using Alexandria.Common.Tests.Interfaces;
using Alexandria.Domain.Common.Entities.Tag;
using ErrorOr;

namespace Alexandria.Infrastructure.Tests.TestUtils.Builders;

public class TagBuilder : IBuilder<Tag>
{
    private string _name = "DefaultTagName";

    public TagBuilder WithName(string name)
    {
        _name = name;
        return this;
    }
    
    public ErrorOr<Tag> Build()
    {
        var tag = Tag.Create(_name);
        return tag.IsError ?
                tag.Errors :
                tag.Value;
    }
}