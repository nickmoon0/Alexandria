using ErrorOr;

namespace Alexandria.Domain.Common.Entities.Tag;

public class Tag : Entity
{
    public string? Name { get; private set; }
    private List<Guid> _taggingIds = [];
    public IReadOnlyList<Guid> TaggingIds => _taggingIds;

    private Tag() { }

    private Tag(string name, Guid? id = null) : base(id ?? Guid.NewGuid())
    {
        Name = name;
    }

    public static ErrorOr<Tag> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrEmpty(name) || name.Length >= 50)
        {
            return TagErrors.InvalidName;
        }
        
        return new Tag(name);
    }
}