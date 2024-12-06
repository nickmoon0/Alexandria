using Alexandria.Domain.Common.Entities.Tag;
using ErrorOr;

namespace Alexandria.Application.Common.Interfaces;

public interface ITaggingService
{
    public Task<ErrorOr<Success>> TagEntity<T>(T entity, Tag tag);
    public Task<ErrorOr<Success>> RemoveTag<T>(T entity, Tag tag);
}