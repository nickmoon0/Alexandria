using Alexandria.Domain.CharacterAggregate;
using Alexandria.Domain.EntryAggregate;
using Alexandria.Infrastructure.Persistence.Models.CharacterEntry;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Alexandria.Infrastructure.Persistence.Configurations;

public class EntryConfiguration : IEntityTypeConfiguration<Entry>
{
    public void Configure(EntityTypeBuilder<Entry> builder)
    {
        builder.ToTable(nameof(Entry));
        
        builder.HasQueryFilter(e => e.DeletedAtUtc == null);
        
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();
        
        // Property Configurations
        builder.Property(e => e.Name)
            .IsRequired();
        
        builder.Property(e => e.CreatedById)
            .IsRequired();
        builder.Property(e => e.CreatedAtUtc)
            .IsRequired();

        // Relationship Configuration with Comments
        builder.HasMany(e => e.Comments)
            .WithOne(c => c.Entry)
            .OnDelete(DeleteBehavior.NoAction);
        
        // One-to-One Relationship with Document
        builder.HasOne(e => e.Document)
            .WithOne()
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasMany(entry => entry.Characters)
            .WithMany(character => character.Entries)
            .UsingEntity<CharacterEntry>(
                entityBuilder => entityBuilder.HasOne<Character>()
                    .WithMany()
                    .HasForeignKey(ce => ce.CharacterId),
                entityBuilder => entityBuilder.HasOne<Entry>()
                    .WithMany()
                    .HasForeignKey(ce => ce.EntryId));
        
        // Create indexes
        builder
            .HasIndex(e => new { e.CreatedAtUtc, e.Id })
            .IsUnique();
    }
}