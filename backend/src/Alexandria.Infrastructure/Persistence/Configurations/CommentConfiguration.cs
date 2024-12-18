using Alexandria.Domain.EntryAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Alexandria.Infrastructure.Persistence.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable(nameof(Comment));
        
        builder.HasQueryFilter(c => c.DeletedAtUtc == null);
        
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();

        builder.Property(c => c.Content)
            .IsRequired();
        
        builder.Property(c => c.CreatedById)
            .IsRequired();
        builder.Property(c => c.CreatedAtUtc)
            .IsRequired();
        
        // Configure the relationship between Comment and Entry
        builder.HasOne(c => c.Entry)
            .WithMany(e => e.Comments)
            .OnDelete(DeleteBehavior.NoAction)
            .IsRequired();
    }
}