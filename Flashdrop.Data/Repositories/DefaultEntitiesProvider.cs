using Flashdrop.Application.Interfaces.Repositories;
using Flashdrop.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flashdrop.Data.Repositories;

/// <summary>
/// Used for getting default entities during development while db is not fully set up
/// </summary>
public class DefaultEntitiesProvider(FlashdropDbContext context) : IDefaultEntitiesProvider
{
    public async Task<User> GetDefaultSellerAsync(CancellationToken cancellationToken = default)
    {
        var seller = await context.Users
            .FirstOrDefaultAsync(u => u.Role == UserRole.Seller, cancellationToken);

        return seller ?? throw new InvalidOperationException(
            "No seeded seller found. Make sure the database seeder has run.");
    }

    public async Task<User> GetDefaultAdminAsync(CancellationToken cancellationToken = default)
    {
        var admin = await context.Users
            .FirstOrDefaultAsync(u => u.Role == UserRole.Admin, cancellationToken);

        return admin ?? throw new InvalidOperationException(
            "No seeded admin found. Make sure the database seeder has run.");
    }
}