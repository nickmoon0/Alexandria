using Alexandria.Application.Tags.Responses;

namespace Alexandria.Api.Tags.DTOs;

public class TagDto
{
    public Guid? Id { get; init; }
    public string? Name { get; init; }

    public static TagDto? FromTagResponse(TagResponse? tagResponse) =>
        tagResponse == null
            ? null
            : new TagDto
            {
                Id = tagResponse.Id,
                Name = tagResponse.Name
            };
}