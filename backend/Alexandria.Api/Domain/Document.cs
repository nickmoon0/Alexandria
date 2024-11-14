using Alexandria.Api.Common;

namespace Alexandria.Api.Domain;

public class Document : IDomainEntity
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; } 
    public byte[]? Data { get; set; }

    public List<Person>? People { get; set; }

    private Document() { }
    
    public Document(string name, string description, byte[] data, List<Person>? people = null)
    {
        Name = name;
        Description = description;
        Data = data;
        People = people ?? [];
    }
}