using Alexandria.Domain.Common.Entities.Tag;
using Alexandria.Infrastructure.Persistence.Models.Tagging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Alexandria.Infrastructure.Persistence.Configurations;

public class TaggingConfigurations : IEntityTypeConfiguration<Tagging>
{
    public void Configure(EntityTypeBuilder<Tagging> builder)
    {
        builder.ToTable(nameof(Tagging));

        builder.HasQueryFilter(tg => tg.DeletedAtUtc == null);
        
        // Primary Key
        builder.HasKey(tg => tg.TaggingId);

        // Properties
        builder.Property(tg => tg.TaggingId)
            .ValueGeneratedOnAdd();
        builder.Property(tg => tg.EntityType)
            .IsRequired();
        builder.Property(tg => tg.EntityId)
            .IsRequired();

        // Foreign Key Relationship
        builder.HasOne<Tag>()
            .WithMany()
            .HasForeignKey(tg => tg.TagId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        // Composite Index for fast lookups
        builder
            .HasIndex(tg => new { tg.TagId, tg.EntityId })
            .IsUnique();
        builder.HasIndex(tg => new { tg.EntityType, tg.EntityId });
        builder.HasIndex(tg => new { tg.TagId });
    }
}