using Alexandria.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Alexandria.Api.Infrastructure.Data;

public static class ConfigureEntityExtensions
{
    public static ModelBuilder ConfigureDocumentEntity(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Document>()
            .ToTable(nameof(Document))
            .Ignore(doc => doc.Data);

        return modelBuilder;
    }

    public static ModelBuilder ConfigurePersonEntity(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>()
            .ToTable(nameof(Person));

        return modelBuilder;
    }
}