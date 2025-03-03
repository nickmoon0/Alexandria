using Alexandria.Domain.Common;
using Alexandria.Domain.Common.Entities.Tag;
using ErrorOr;

namespace Alexandria.Application.Common.Interfaces;

public interface ITaggingService
{
    public Task<ErrorOr<Success>> TagEntity<T>(T entity, Tag tag) where T : Entity;
    public Task<ErrorOr<Deleted>> RemoveTag<T>(T entity, Tag tag) where T : Entity;
    public Task<ErrorOr<IReadOnlyList<Tag>>> GetEntityTags<TEntity>(TEntity entity, CancellationToken cancellationToken) 
        where TEntity : Entity;
    
    /// <summary>
    /// This method asynchronously retrieves and groups tag data for a list of entities.
    /// It extracts the entity IDs, fetches associated tagging records and unique Tag objects from the database,
    /// and then returns a read-only dictionary mapping each entity's Guid to its collection of Tag objects
    /// (entities without tags are omitted).
    /// </summary>
    /// <param name="entities"></param>
    /// <param name="cancellationToken"></param>
    /// <typeparam name="TEntity"></typeparam>
    /// <returns></returns>
    public Task<IReadOnlyDictionary<Guid, IEnumerable<Tag>>> GetEntitiesTags<TEntity>(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken) where TEntity : Entity;
}