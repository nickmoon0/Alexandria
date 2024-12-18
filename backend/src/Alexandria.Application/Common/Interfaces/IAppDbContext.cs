using Alexandria.Domain.CharacterAggregate;
using Alexandria.Domain.Common.Entities.Tag;
using Alexandria.Domain.EntryAggregate;
using Alexandria.Domain.UserAggregate;
using Microsoft.EntityFrameworkCore;

namespace Alexandria.Application.Common.Interfaces;

public interface IAppDbContext
{
    public DbSet<Character> Characters { get; }
    public DbSet<Comment> Comments { get; } 
    public DbSet<Document> Documents { get; }
    public DbSet<Entry> Entries { get; }
    public DbSet<Tag> Tags { get; }
    public DbSet<User> Users { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}