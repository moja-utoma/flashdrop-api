using Microsoft.Extensions.DependencyInjection;

namespace Flashdrop.Data.Extensions;

/// <summary>
/// Extension methods for database seeding in dependency injection setup.
/// </summary>
public static class SeedingExtensions
{
    /// <summary>
    /// Adds the database seeder to the dependency injection container.
    /// </summary>
    public static IServiceCollection AddDatabaseSeeder(this IServiceCollection services)
    {
        services.AddScoped<DatabaseSeeder>();
        return services;
    }

    /// <summary>
    /// Seeds the database with initial test data asynchronously.
    /// Useful for development and testing environments.
    /// </summary>
    public static async Task SeedDatabaseAsync(
       this IServiceProvider serviceProvider,
       CancellationToken cancellationToken = default)
    {
        using var scope = serviceProvider.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedAsync(cancellationToken);
    }
}
