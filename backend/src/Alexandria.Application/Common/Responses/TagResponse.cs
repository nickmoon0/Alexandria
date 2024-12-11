using Alexandria.Domain.Common.Entities.Tag;

namespace Alexandria.Application.Common.Responses;

public class TagResponse
{
    public Guid? Id { get; init; }
    public string? Name { get; init; }

    public static TagResponse FromTag(Tag tag)
    {
        return new TagResponse
        {
            Id = tag.Id,
            Name = tag.Name
        };
    }
}