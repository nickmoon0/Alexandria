using Alexandria.Common.Tests.Interfaces;
using Alexandria.Infrastructure.Tests.TestUtils.Services;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Alexandria.Infrastructure.Tests.TestUtils.Builders;

public class DbContextBuilder<TContext> : IBuilder<TContext> where TContext : DbContext
{
    private string _connectionString = "DataSource=:memory:";
    private IPublisher _publisher = new MockPublisher();

    public DbContextBuilder<TContext> WithConnectionString(string connectionString)
    {
        _connectionString = connectionString;
        return this;
    }

    public DbContextBuilder<TContext> WithPublisher(IPublisher publisher)
    {
        _publisher = publisher;
        return this;
    }

    public ErrorOr<TContext> Build()
    {
        var options = new DbContextOptionsBuilder<TContext>()
            .UseSqlite(_connectionString)
            .Options;

        if (options == null)
        {
            return Error.Failure();
        }

        var dbContext = (TContext)Activator.CreateInstance(typeof(TContext), options, _publisher)!;

        // Open the SQLite connection and ensure the database schema is created
        dbContext.Database.OpenConnection();
        dbContext.Database.EnsureCreated();

        return dbContext;
    }
}
