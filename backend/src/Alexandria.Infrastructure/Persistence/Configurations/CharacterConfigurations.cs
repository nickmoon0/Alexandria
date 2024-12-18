using Alexandria.Domain.CharacterAggregate;
using Alexandria.Domain.Common.ValueObjects.Name;
using Alexandria.Domain.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Alexandria.Infrastructure.Persistence.Configurations;

public class CharacterConfigurations : IEntityTypeConfiguration<Character>
{
    public void Configure(EntityTypeBuilder<Character> builder)
    {
        builder.ToTable(nameof(Character));

        builder.HasQueryFilter(x => x.DeletedAtUtc == null);

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever();
        
        builder.Property(x => x.CreatedById)
            .IsRequired();
        
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.CreatedById);
        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(x => x.UserId);
        
        builder.OwnsOne(character => character.Name, nameBuilder =>
        {
            nameBuilder.Property(name => name.FirstName)
                .HasColumnName(nameof(Name.FirstName))
                .IsRequired();
            nameBuilder.Property(name => name.LastName)
                .HasColumnName(nameof(Name.LastName))
                .IsRequired();
            
            nameBuilder.Property(name => name.MiddleNames)
                .HasColumnName(nameof(Name.MiddleNames));
        });
    }
}