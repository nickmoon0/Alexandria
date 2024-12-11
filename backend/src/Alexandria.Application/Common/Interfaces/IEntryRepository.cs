using Alexandria.Domain.EntryAggregate;
using ErrorOr;

namespace Alexandria.Application.Common.Interfaces;

public interface IEntryRepository
{
    public Task<ErrorOr<Success>> AddAsync(Entry entry, Document document, CancellationToken cancellationToken);
    public Task<ErrorOr<Entry>> FindByIdAsync(Guid entryId, CancellationToken cancellationToken);
    public Task<ErrorOr<Success>> UpdateAsync(CancellationToken cancellationToken);
}