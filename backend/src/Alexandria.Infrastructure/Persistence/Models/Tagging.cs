using Alexandria.Domain.Common.Entities.Tag;

namespace Alexandria.Infrastructure.Persistence.Models;

public class Tagging
{
    public Guid TaggingId { get; set; }
    public Guid TagId { get; set; }
    public required string EntityType { get; set; }
    public Guid EntityId { get; set; }
}