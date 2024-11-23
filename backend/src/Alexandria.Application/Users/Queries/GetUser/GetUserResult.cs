namespace Alexandria.Application.Users.Queries.GetUser;

public class GetUserResult
{
    public required Guid Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? MiddleNames { get; set; }
}