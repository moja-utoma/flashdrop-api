using Flashdrop.Application.Interfaces.Repositories;
using Flashdrop.Domain.Entities;
using Flashdrop.Infrastructure.Data;
using Flashdrop.Infrastructure.Data.Extensions;
using Flashdrop.Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Flashdrop.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration);

        services.AddRepositories();
        services.AddDatabaseSeeder();

        services.AddRedisCache(configuration);
        services.AddRabbitMq(configuration);

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<FlashdropDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions => sqlOptions.MigrationsAssembly("Flashdrop.Infrastructure"));
        });

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IRepository<Product>, ProductRepository>();
        services.AddScoped<IRepository<Sale>, SaleRepository>();
        services.AddScoped<IDefaultEntitiesProvider, DefaultEntitiesProvider>();

        return services;
    }

    private static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
        });

        return services;
    }

    private static IServiceCollection AddRabbitMq(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IConnectionFactory>(_ =>
        {
            var section = configuration.GetSection("RabbitMq");
            return new ConnectionFactory
            {
                HostName = section["HostName"],
                Port = int.Parse(section["Port"]!),
                UserName = section["UserName"],
                Password = section["Password"]
            };
        });

        return services;
    }

}
