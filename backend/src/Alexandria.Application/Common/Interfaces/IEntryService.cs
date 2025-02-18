using Alexandria.Domain.CharacterAggregate;
using Alexandria.Domain.EntryAggregate;
using ErrorOr;

namespace Alexandria.Application.Common.Interfaces;

public interface IEntryService
{
    public ErrorOr<Success> AddCharacterToEntry(Entry entry, Character character);
    public ErrorOr<Success> RemoveCharacterFromEntry(Entry entry, Character character);
}