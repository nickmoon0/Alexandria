using Alexandria.Domain.Common;

namespace Alexandria.Domain.UserAggregate.Events;

public record UserDeletedEvent(Guid UserId) : IDomainEvent;