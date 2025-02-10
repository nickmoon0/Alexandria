using Alexandria.Application.Users.Responses;

namespace Alexandria.CoreApi.Users.DTOs;

public class UserDto
{
    public Guid? Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MiddleNames { get; set; }

    public static UserDto? FromUserResponse(UserResponse? userResponse) =>
        userResponse == null 
            ? null 
            : new UserDto 
            { 
                Id = userResponse.Id, 
                FirstName = userResponse.Name.FirstName, 
                LastName = userResponse.Name.LastName, 
                MiddleNames = userResponse.Name.MiddleNames 
            };
}