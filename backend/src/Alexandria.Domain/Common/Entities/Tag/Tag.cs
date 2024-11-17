using ErrorOr;

namespace Alexandria.Domain.Common.Entities.Tag;

public class Tag : Entity
{
    private string? _name;
    
    private Tag() { }

    private Tag(string name, Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        _name = name;
    }

    public static ErrorOr<Tag> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrEmpty(name) || name.Length >= 20)
        {
            return TagErrors.InvalidName;
        }
        
        return new Tag(name);
    }
}