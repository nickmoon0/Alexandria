using ErrorOr;

namespace Alexandria.Domain.Common;

public abstract class TaggableAggregateRoot : AggregateRoot
{
    private readonly List<Guid> _tags = [];
    public IReadOnlyCollection<Guid> Tags => _tags;

    protected TaggableAggregateRoot() { }
    protected TaggableAggregateRoot(Guid id) : base(id) { }

    public ErrorOr<Updated> AddTag(Guid tagId)
    {
        if (tagId == Guid.Empty)
        {
            return Error.Validation();
        }
        if (_tags.Contains(tagId))
        {
            return Error.Conflict();
        }
        
        _tags.Add(tagId);
        return Result.Updated;
    }

    public ErrorOr<Updated> RemoveTag(Guid tagId)
    {
        if (tagId == Guid.Empty)
        {
            return Error.Validation();
        }

        if (!_tags.Contains(tagId))
        {
            return Error.NotFound();
        }
        
        _tags.Remove(tagId);
        return Result.Updated;
    }
}