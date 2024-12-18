using Alexandria.Domain.Common.Entities.Tag;

namespace Alexandria.Application.Tags.Responses;

public class TagResponse
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }

    public static TagResponse FromTag(Tag tag)
    {
        return new TagResponse
        {
            Id = tag.Id,
            Name = tag.Name
        };
    }
}