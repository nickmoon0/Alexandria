using Alexandria.Api.Common;

namespace Alexandria.Api.Domain;

public class Person : IDomainEntity
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }

    public List<Document>? Documents { get; set; }
    
    private Person() { }
    
    public Person(string name, string description, List<Document>? documents = null)
    {
        Name = name;
        Description = description;
        Documents = documents ?? [];
    }
}