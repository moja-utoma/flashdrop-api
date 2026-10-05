using Flashdrop.Application.Interfaces.Repositories;
using Flashdrop.Application.Products;
using Flashdrop.Data;
using Flashdrop.Data.Entities;
using Flashdrop.Data.Extensions;
using Flashdrop.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;

namespace Flashdrop.API.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration);
        services.AddSwaggerDocumentation();
        services.AddRedisCache(configuration);
        services.AddRabbitMq(configuration);
        services.AddMappings();
        services.AddRepositories();
        services.AddBusinessServices();
        services.AddDatabaseSeeder();

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<FlashdropDbContext>(options =>
        {
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlOptions => sqlOptions.MigrationsAssembly("Flashdrop.Data"));
        });

        return services;
    }

    private static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddOpenApi();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

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

    private static IServiceCollection AddMappings(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => { }, typeof(ProductsProfile).Assembly);

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IRepository<Product>, ProductRepository>();
        services.AddScoped<IDefaultEntitiesProvider, DefaultEntitiesProvider>();

        return services;
    }

    private static IServiceCollection AddBusinessServices(this IServiceCollection services)
    {
        services.AddScoped<IProductService, ProductService>();

        return services;
    }
}