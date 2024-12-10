using Alexandria.Application.Common.Interfaces;
using Alexandria.Domain.EntryAggregate;
using Alexandria.Domain.EntryAggregate.Errors;
using ErrorOr;

namespace Alexandria.Infrastructure.Persistence.Repositories;

public class EntryRepository(AppDbContext context) : IEntryRepository
{
    public async Task<ErrorOr<Success>> AddAsync(Entry entry, Document document, CancellationToken cancellationToken)
    {
        await context.Entries.AddAsync(entry, cancellationToken);
        await context.Documents.AddAsync(document, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return Result.Success;
    }
    
    public async Task<ErrorOr<Entry>> FindByIdAsync(Guid entryId, CancellationToken cancellationToken)
    {
        var entry = await context.Entries.FindAsync([entryId], cancellationToken);

        if (entry == null)
        {
            return EntryErrors.NotFound;
        }
        
        return entry;
    }

    public async Task<ErrorOr<Success>> UpdateAsync(CancellationToken cancellationToken)
    {
        await context.SaveChangesAsync(cancellationToken);
        return Result.Success;
    }
}