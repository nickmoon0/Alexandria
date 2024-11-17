namespace Alexandria.Domain.Common;

public abstract class AggregateRoot : Entity
{
    protected AggregateRoot() { }
    protected AggregateRoot(Guid id) : base(id) { }
    
    protected readonly List<IDomainEvent> DomainEvents = [];

    public List<IDomainEvent> PopDomainEvents()
    {
        var copy = DomainEvents.ToList();
        DomainEvents.Clear();
        
        return copy;
    }
}