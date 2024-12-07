using Alexandria.Domain.Common;
using Alexandria.Domain.Common.Entities.Tag;
using ErrorOr;

namespace Alexandria.Application.Common.Interfaces;

public interface ITaggingService
{
    public Task<ErrorOr<Success>> TagEntity<T>(T entity, Tag tag) where T : Entity;
    public Task<ErrorOr<Deleted>> RemoveTag<T>(T entity, Tag tag) where T : Entity;
}