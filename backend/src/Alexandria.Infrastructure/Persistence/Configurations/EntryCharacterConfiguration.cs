using Alexandria.Domain.CharacterAggregate;
using Alexandria.Domain.EntryAggregate;
using Alexandria.Infrastructure.Persistence.Models.EntryCharacter;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Alexandria.Infrastructure.Persistence.Configurations;

public class EntryCharacterConfiguration : IEntityTypeConfiguration<EntryCharacter>
{
    public void Configure(EntityTypeBuilder<EntryCharacter> builder)
    {
        builder.HasKey(ec => ec.EntryCharacterId);

        builder
            .HasOne<Entry>()
            .WithMany()
            .HasForeignKey(ec => ec.EntryId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder
            .HasOne<Character>()
            .WithMany()
            .HasForeignKey(ec => ec.CharacterId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}