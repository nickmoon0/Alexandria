using Alexandria.Api.Common.Interfaces;
using Alexandria.Api.Common.Models;

namespace Alexandria.Api.Domain;

public class Person : IDomainEntity
{
    public Guid Id { get; set; }
    public string? FirstName { get; private set; }
    public string? LastName { get; private set; }
    public string? MiddleNames { get; private set; }
    public string? Description { get; private set; }
    
    public List<Document>? Documents { get; private set; }
    
    private Person() { }

    public static Result<Person> Create(
        string firstName,
        string lastName,
        string? middleNames = null,
        string? description = null)
    {
        var person = new Person
        {
            FirstName = firstName,
            LastName = lastName,
            MiddleNames = middleNames,
            Description = description,
            Documents = []
        };

        if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
        {
            var error = new Error
            {
                Description = "First name and last name must have values",
                Type = ErrorType.EntityValidationFailed
            };
            return Result<Person>.CreateErrorResult(error);
        }
        
        var result = Result<Person>.CreateSuccessResult(person);
        return result;
    }
    
    public Result<IEnumerable<Document>> AddDocument(Document document)
    {
        throw new NotImplementedException();
    }
    public Result<IEnumerable<Document>> AddDocuments(IEnumerable<Document> documents)
    {
        throw new NotImplementedException();
    }
}