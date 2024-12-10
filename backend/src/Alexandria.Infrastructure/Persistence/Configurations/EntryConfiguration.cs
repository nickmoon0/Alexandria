using Alexandria.Domain.EntryAggregate;
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
    }
}