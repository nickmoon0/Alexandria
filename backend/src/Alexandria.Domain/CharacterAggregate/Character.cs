using Alexandria.Domain.Common;
using Alexandria.Domain.Common.ValueObjects.Name;
using ErrorOr;

namespace Alexandria.Domain.CharacterAggregate;

public class Character : AggregateRoot
{
    private Name? _name;
    private string? _description;

    private Guid? _userId;
    
    private Character() { }

    private Character(Name name, string? description, Guid? userId, Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        _name = name;
        _description = description;
        _userId = userId;
    }

    public static ErrorOr<Character> Create(Name name, string? description = null, Guid? userId = null)
    {
        var errors = new List<Error>();
        
        // Check description is in valid state
        description = description?.Trim();
        if (!string.IsNullOrEmpty(description) && description.Length > 2000)
        {
            errors.Add(CharacterErrors.DescriptionTooLong);
        }
        
        // Check userId is in valid state
        if (userId == Guid.Empty)
        {
            errors.Add(CharacterErrors.InvalidUserId);
        }
        
        if (errors.Count != 0)
        {
            return errors;
        }
        
        return new Character(name, description, userId);
    }
}