using Alexandria.Domain.EntryAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Alexandria.Infrastructure.Persistence.Configurations;

public class DocumentConfiguration : IEntityTypeConfiguration<Document>
{
    public void Configure(EntityTypeBuilder<Document> builder)
    {
        builder.ToTable(nameof(Document));
        
        builder.HasQueryFilter(x => x.DeletedAtUtc == null);
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();

        builder.Property(x => x.Name)
            .IsRequired();
        builder.Property(x => x.FileExtension)
            .IsRequired();
        builder.Property(x => x.ImagePath)
            .IsRequired();
        builder.Property(x => x.CreatedById)
            .IsRequired();
        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();
        
        builder.Property(x => x.EntryId)
            .IsRequired();
        
        // One-to-One Relationship with Entry
        builder.HasOne<Entry>()
            .WithOne(e => e.Document)
            .HasForeignKey<Document>(d => d.EntryId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}