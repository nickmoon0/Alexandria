namespace Alexandria.Api.Features.People.DTOs;

public class PersonDto
{
    public required Guid Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? MiddleNames { get; set; }
    public string? Description { get; set; }
}