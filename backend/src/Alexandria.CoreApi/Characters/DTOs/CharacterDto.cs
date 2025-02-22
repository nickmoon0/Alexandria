using Alexandria.Application.Characters.Responses;
using Alexandria.CoreApi.Users.DTOs;

namespace Alexandria.CoreApi.Characters.DTOs;

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

    public static CharacterDto FromCharacterResponse(CharacterResponse? characterResponse) =>
        characterResponse == null
            ? null
            : new CharacterDto
            {
                Id = characterResponse.Id,
                FirstName = characterResponse.Name.FirstName,
                LastName = characterResponse.Name.LastName,
                MiddleNames = characterResponse.Name.MiddleNames,
                Description = characterResponse.Description,
                User = UserDto.FromUserResponse(characterResponse.User),
                CreatedBy = UserDto.FromUserResponse(characterResponse.CreatedBy),
                CreatedOnUtc = characterResponse.CreatedAtUtc
            };
}