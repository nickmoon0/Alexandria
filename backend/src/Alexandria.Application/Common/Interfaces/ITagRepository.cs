using Alexandria.Domain.Common;
using Alexandria.Domain.Common.Entities.Tag;
using ErrorOr;

namespace Alexandria.Application.Common.Interfaces;

public interface ITagRepository
{
    public Task<ErrorOr<Tag>> FindByIdAsync(Guid tagId, CancellationToken cancellationToken);
    public Task<ErrorOr<IReadOnlyList<Tag>>> GetEntityTags<TEntity>(TEntity entity, CancellationToken cancellationToken) 
        where TEntity : Entity;

    public Task<ErrorOr<Success>> AddEntityTag<TEntity>(TEntity entity, Tag tag, CancellationToken cancellationToken)
        where TEntity : Entity;
}