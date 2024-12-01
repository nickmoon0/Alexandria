using Alexandria.Domain.Common.ValueObjects.Name;
using Alexandria.Domain.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Alexandria.Infrastructure.Persistence.Configurations;

public class UserConfigurations : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable(nameof(User));
        
        builder.HasQueryFilter(x => x.DeletedAtUtc == null); // Only get objects that haven't been deleted
        
        builder.HasKey(user => user.Id);
        builder.Property(user => user.Id).ValueGeneratedNever();
        
        builder.OwnsOne(user => user.Name, nameBuilder =>
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