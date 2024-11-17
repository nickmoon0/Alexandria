using Alexandria.Domain.Common;
using Alexandria.Domain.Common.ValueObjects.Name;
using ErrorOr;

namespace Alexandria.Domain.CharacterAggregate;

public class Character : AggregateRoot
{
    public Name Name { get; }
    public string? Description { get; }
    
    private Character() { }

    private Character(Guid id, Name name, string? description) : base(id)
    {
        Name = name;
        Description = description;
    }

    public static ErrorOr<Character> Create(Name name, string? description = null)
    {
        var errors = new List<Error>();
        
        description = description?.Trim();
        if (!string.IsNullOrEmpty(description) && description.Length > 2000)
        {
            errors.Add(CharacterErrors.DescriptionTooLong);
        }

        if (errors.Count != 0)
        {
            return errors;
        }
        
        return new Character(Guid.NewGuid(), name, description);
    }
}