using Alexandria.Domain.Common.Entities.Tag;
using ErrorOr;

namespace Alexandria.Application.Common.Interfaces;

public interface ITagRepository
{
    public Task<ErrorOr<Success>> AddAsync(Tag tag, CancellationToken cancellationToken);
    public Task<ErrorOr<Tag>> FindByIdAsync(Guid tagId, CancellationToken cancellationToken);
    public Task<ErrorOr<IEnumerable<Tag>>> GetAllTags(CancellationToken cancellationToken);
}