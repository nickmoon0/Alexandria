using Alexandria.Domain.Common;

namespace Alexandria.Domain.UserAggregate.Events;

public record UserCreatedEvent(User User) : IDomainEvent;