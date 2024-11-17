using Alexandria.Api.Common.Interfaces;

namespace Alexandria.Api.Domain;

public class Person : IDomainEntity
{
    // Domain entity properties
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MiddleNames { get; set; }
    public string? Description { get; set; }
    
    public List<Document>? Documents { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    private Person() { }

    public Person(
        string firstName,
        string lastName,
        string? middleNames = null,
        string? description = null)
    {
        FirstName = firstName;
        LastName = lastName;
        MiddleNames = middleNames;
        Description = description;
        Documents = [];
        IsDeleted = false;
        CreatedAtUtc = DateTime.UtcNow;
    }
}