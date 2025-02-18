using Alexandria.Application.Common.Interfaces;
using Alexandria.Domain.CharacterAggregate;
using Alexandria.Domain.EntryAggregate;
using Alexandria.Infrastructure.Persistence;
using ErrorOr;
using Microsoft.Extensions.Logging;

namespace Alexandria.Infrastructure.Services;

public class EntryService : IEntryService
{
    private readonly AppDbContext _context;
    private readonly ILogger<EntryService> _logger;
    
    public ErrorOr<Success> AddCharacterToEntry(Entry entry, Character character)
    {
        throw new NotImplementedException();
    }

    public ErrorOr<Success> RemoveCharacterFromEntry(Entry entry, Character character)
    {
        throw new NotImplementedException();
    }
}