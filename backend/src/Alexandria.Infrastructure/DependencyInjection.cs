using Alexandria.Application.Common.Interfaces;
using Alexandria.Domain.Common.Interfaces;
using Alexandria.Infrastructure.Persistence;
using Alexandria.Infrastructure.Persistence.Repositories;
using Alexandria.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Alexandria.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddMediatR(options => 
                options.RegisterServicesFromAssemblyContaining(typeof(DependencyInjection)))
            .AddPersistence(configuration)
            .AddServices();

        return services;
    }
    
    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var sqlConnString = configuration.GetConnectionString(nameof(AppDbContext));
        var serverVersion = ServerVersion.AutoDetect(sqlConnString);
        
        services.AddDbContext<AppDbContext>(options => options.UseMySql(
            sqlConnString,
            serverVersion,
            mySqlOptionsAction: mysqlOptions => 
                mysqlOptions.MigrationsAssembly(typeof(AppDbContext).Assembly.GetName().Name)));
        
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICharacterRepository, CharacterRepository>();
        
        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddHostedService<RabbitMqConsumerService>();
        services.AddScoped<MessageProcessorService>();
        
        services.AddScoped<IDateTimeProvider, SystemDateTimeProvider>();
        
        return services;
    }
}