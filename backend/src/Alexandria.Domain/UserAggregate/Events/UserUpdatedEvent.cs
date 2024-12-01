using Alexandria.Domain.Common;

namespace Alexandria.Domain.UserAggregate.Events;

public record UserUpdatedEvent(User User) : IDomainEvent;