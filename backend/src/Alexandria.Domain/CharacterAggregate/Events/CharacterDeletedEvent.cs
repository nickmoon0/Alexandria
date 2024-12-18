using Alexandria.Domain.Common;

namespace Alexandria.Domain.CharacterAggregate.Events;

public record CharacterDeletedEvent(Guid CharacterId) : IDomainEvent;