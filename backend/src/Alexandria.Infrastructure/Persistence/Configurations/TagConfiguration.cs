using Alexandria.Domain.Common.Entities.Tag;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Alexandria.Infrastructure.Persistence.Configurations;

public class TagConfigurations : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable(nameof(Tag));

        // Primary Key
        builder.HasKey(t => t.Id);

        // Properties
        builder.Property(t => t.Id)
            .ValueGeneratedNever();
        
        builder.Property(t => t.Name)
            .IsRequired();

        // Unique Constraint
        builder.HasIndex(t => t.Name)
            .IsUnique();
    }
}