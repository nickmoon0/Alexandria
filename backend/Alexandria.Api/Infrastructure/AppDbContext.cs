using Alexandria.Api.Domain;
using Alexandria.Api.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Alexandria.Api.Infrastructure;

public class AppDbContext : DbContext
{
    public DbSet<Document> Documents { get; set; }
    public DbSet<Person> People { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .ConfigureDocumentEntity()
            .ConfigurePersonEntity();
        
        base.OnModelCreating(modelBuilder);
    }
}