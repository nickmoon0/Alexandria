using Alexandria.Application.Tags.Responses;
using Alexandria.Application.Users.Responses;
using Alexandria.Domain.Common.ValueObjects.Name;

namespace Alexandria.Application.Characters.Responses;

public class CharacterResponse
{
    public Guid? Id { get; set; }
    public Name? Name { get; set; }

    public string? Description { get; set; }
    public IReadOnlyList<TagResponse>? Tags { get; set; }
    public UserResponse? User { get; set; }
    public UserResponse? CreatedBy { get; set; }

    public DateTime? CreatedAtUtc { get; set; }
}