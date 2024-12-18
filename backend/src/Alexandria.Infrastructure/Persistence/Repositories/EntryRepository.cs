using Alexandria.Application.Common.Interfaces;
using Alexandria.Domain.EntryAggregate;
using Alexandria.Domain.EntryAggregate.Errors;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

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

    public async Task<ErrorOr<Entry>> FindByIdAsync(
        Guid entryId, CancellationToken cancellationToken, HashSet<FindOptions>? optionsList = null)
    {
        IQueryable<Entry> entryQuery = context.Entries;

        if (optionsList?.Contains(FindOptions.IncludeDocument) ?? false)
        {
            entryQuery = entryQuery.Include(entry => entry.Document);
        }

        if (optionsList?.Contains(FindOptions.IncludeComments) ?? false)
        {
            entryQuery = entryQuery.Include(entry => entry.Comments);
        }

        var entry = await entryQuery.SingleOrDefaultAsync(x => x.Id == entryId, cancellationToken);
        
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