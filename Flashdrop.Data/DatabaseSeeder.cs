using Flashdrop.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flashdrop.Data;

public class DatabaseSeeder(FlashdropDbContext context)
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        // Only seed if database is empty
        if (await context.Users.AnyAsync(cancellationToken) || await context.Products.AnyAsync(cancellationToken))
        {
            return;
        }

        var sellers = new[]
        {
            new User
            {
                Id = Guid.CreateVersion7(),
                Email = "seller1@example.com",
                PasswordHash = "hashed_password_1",
                Role = UserRole.Seller,
                CreatedAt = DateTimeOffset.UtcNow,
                IsActive = true
            },
            new User
            {
                Id = Guid.CreateVersion7(),
                Email = "seller2@example.com",
                PasswordHash = "hashed_password_2",
                Role = UserRole.Seller,
                CreatedAt = DateTimeOffset.UtcNow,
                IsActive = true
            },
            new User
            {
                Id = Guid.CreateVersion7(),
                Email = "admin@example.com",
                PasswordHash = "hashed_password_admin",
                Role = UserRole.Admin,
                CreatedAt = DateTimeOffset.UtcNow,
                IsActive = true
            }
        };

        var customers = new[]
        {
            new User
            {
                Id = Guid.CreateVersion7(),
                Email = "customer1@example.com",
                PasswordHash = "hashed_password_c1",
                Role = UserRole.Customer,
                CreatedAt = DateTimeOffset.UtcNow,
                IsActive = true
            },
            new User
            {
                Id = Guid.CreateVersion7(),
                Email = "customer2@example.com",
                PasswordHash = "hashed_password_c2",
                Role = UserRole.Customer,
                CreatedAt = DateTimeOffset.UtcNow,
                IsActive = true
            }
        };

        var allUsers = sellers.Concat(customers).ToList();
        await context.Users.AddRangeAsync(allUsers, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var products = new[]
        {
            new Product
            {
                Id = Guid.CreateVersion7(),
                SellerId = sellers[0].Id,
                Name = "Wireless Headphones",
                Description = "High-quality Bluetooth wireless headphones with noise cancellation",
                ImageUrl = "https://example.com/headphones.jpg",
                BasePrice = 99.99m,
                Category = ProductCategory.Electronics,
                CreatedAt = DateTimeOffset.UtcNow
            },
            new Product
            {
                Id = Guid.CreateVersion7(),
                SellerId = sellers[0].Id,
                Name = "USB-C Cable",
                Description = "Durable USB-C charging and data transfer cable",
                ImageUrl = "https://example.com/usb-c-cable.jpg",
                BasePrice = 12.99m,
                Category = ProductCategory.Electronics,
                CreatedAt = DateTimeOffset.UtcNow
            },
            new Product
            {
                Id = Guid.CreateVersion7(),
                SellerId = sellers[1].Id,
                Name = "Cotton T-Shirt",
                Description = "Comfortable 100% cotton crew neck t-shirt",
                ImageUrl = "https://example.com/tshirt.jpg",
                BasePrice = 24.99m,
                Category = ProductCategory.Fashion,
                CreatedAt = DateTimeOffset.UtcNow
            },
            new Product
            {
                Id = Guid.CreateVersion7(),
                SellerId = sellers[1].Id,
                Name = "Running Shoes",
                Description = "Professional grade running shoes with excellent cushioning",
                ImageUrl = "https://example.com/shoes.jpg",
                BasePrice = 129.99m,
                Category = ProductCategory.Sports,
                CreatedAt = DateTimeOffset.UtcNow
            },
            new Product
            {
                Id = Guid.CreateVersion7(),
                SellerId = sellers[0].Id,
                Name = "Coffee Maker",
                Description = "Programmable coffee maker with thermal carafe",
                ImageUrl = "https://example.com/coffee-maker.jpg",
                BasePrice = 79.99m,
                Category = ProductCategory.HomeAndLiving,
                CreatedAt = DateTimeOffset.UtcNow
            }
        };

        await context.Products.AddRangeAsync(products, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var sales = new[]
        {
            new Sale
            {
                Id = Guid.CreateVersion7(),
                ProductId = products[0].Id,
                TotalStock = 50,
                AvailableStock = 50,
                PricePerUnit = 74.99m,
                MaxUnitsPerUser = 2,
                StartsAt = DateTimeOffset.UtcNow.AddHours(1),
                EndsAt = DateTimeOffset.UtcNow.AddHours(4),
                Status = SaleStatus.Scheduled
            },
            new Sale
            {
                Id = Guid.CreateVersion7(),
                ProductId = products[1].Id,
                TotalStock = 100,
                AvailableStock = 100,
                PricePerUnit = 8.99m,
                MaxUnitsPerUser = 10,
                StartsAt = DateTimeOffset.UtcNow.AddHours(2),
                EndsAt = DateTimeOffset.UtcNow.AddHours(5),
                Status = SaleStatus.Scheduled
            },
            new Sale
            {
                Id = Guid.CreateVersion7(),
                ProductId = products[2].Id,
                TotalStock = 75,
                AvailableStock = 45,
                PricePerUnit = 17.99m,
                MaxUnitsPerUser = 3,
                StartsAt = DateTimeOffset.UtcNow.Subtract(TimeSpan.FromHours(1)),
                EndsAt = DateTimeOffset.UtcNow.AddHours(2),
                Status = SaleStatus.Live
            },
            new Sale
            {
                Id = Guid.CreateVersion7(),
                ProductId = products[3].Id,
                TotalStock = 30,
                AvailableStock = 30,
                PricePerUnit = 99.99m,
                MaxUnitsPerUser = 1,
                StartsAt = DateTimeOffset.UtcNow.AddHours(6),
                EndsAt = DateTimeOffset.UtcNow.AddHours(10),
                Status = SaleStatus.Scheduled
            },
            new Sale
            {
                Id = Guid.CreateVersion7(),
                ProductId = products[4].Id,
                TotalStock = 20,
                AvailableStock = 15,
                PricePerUnit = 59.99m,
                MaxUnitsPerUser = 1,
                StartsAt = DateTimeOffset.UtcNow.Subtract(TimeSpan.FromHours(2)),
                EndsAt = DateTimeOffset.UtcNow.AddHours(1),
                Status = SaleStatus.Live
            }
        };

        await context.Sales.AddRangeAsync(sales, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }
}