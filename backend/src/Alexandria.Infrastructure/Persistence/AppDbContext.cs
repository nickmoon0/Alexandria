using System.Reflection;
using Alexandria.Application.Common.Interfaces;
using Alexandria.Domain.CharacterAggregate;
using Alexandria.Domain.Common;
using Alexandria.Domain.Common.Entities.Tag;
using Alexandria.Domain.EntryAggregate;
using Alexandria.Domain.UserAggregate;
using Alexandria.Infrastructure.Persistence.Models.Tagging;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Alexandria.Infrastructure.Persistence;

public class AppDbContext(
    DbContextOptions<AppDbContext> options,
    IPublisher publisher) : DbContext(options), IAppDbContext
{
    public virtual DbSet<Character> Characters { get; init; }
    public virtual DbSet<Comment> Comments { get; init; } 
    public virtual DbSet<Document> Documents { get; init; }
    public virtual DbSet<Entry> Entries { get; init; }
    public virtual DbSet<Tag> Tags { get; init; }
    public virtual DbSet<Tagging> Taggings { get; init; }
    public virtual DbSet<User> Users { get; init; }

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var domainEvents = ChangeTracker.Entries<AggregateRoot>()
            .Select(entry => entry.Entity.PopDomainEvents())
            .SelectMany(x => x)
            .ToList();

        await PublishDomainEvents(domainEvents);
        return await base.SaveChangesAsync(cancellationToken);
    }
    
    private async Task PublishDomainEvents(List<IDomainEvent> domainEvents)
    {
        foreach (var domainEvent in domainEvents)
        {
            await publisher.Publish(domainEvent);
        }
    }
}