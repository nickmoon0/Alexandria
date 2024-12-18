using Alexandria.Api.Users.DTOs;

namespace Alexandria.Api.Characters.DTOs;

public class CharacterDto
{
    public Guid? Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MiddleNames { get; set; }
    public string? Description { get; set; }
    public UserDto? User { get; set; }

    public UserDto? CreatedBy { get; set; }
    public DateTime? CreatedOnUtc { get; set; }
}