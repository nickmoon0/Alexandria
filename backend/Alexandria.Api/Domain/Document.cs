using Alexandria.Api.Common;
using Alexandria.Api.Common.Interfaces;

namespace Alexandria.Api.Domain;

public class Document : IDomainEntity
{
    // EF Core mapped fields
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; } 
    public List<Person>? People { get; set; }

    // EF Core unmapped fields
    public byte[]? Data { get; set; }
    
    private Document() { }
    
    public Document(string name, string description, byte[] data, List<Person>? people = null)
    {
        Name = name;
        Description = description;
        Data = data;
        People = people ?? [];
    }
}