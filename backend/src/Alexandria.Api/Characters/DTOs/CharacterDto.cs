namespace Alexandria.Api.Characters.DTOs;

public class CharacterDto
{
    public required Guid Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required Guid CreatedBy { get; set; }
    public required DateTime CreatedOnUtc { get; set; }
    public string? MiddleNames { get; set; }
    public string? Description { get; set; }
    public Guid? UserId { get; set; }
}