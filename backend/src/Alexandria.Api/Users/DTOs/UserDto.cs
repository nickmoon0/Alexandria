namespace Alexandria.Api.Users.DTOs;

public class UserDto
{
    public required Guid Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? MiddleNames { get; set; }
}