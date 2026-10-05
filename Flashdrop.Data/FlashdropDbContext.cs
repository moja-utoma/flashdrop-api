using Flashdrop.Data.Configuration;
using Flashdrop.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Flashdrop.Data;

public class FlashdropDbContext : DbContext
{
    public FlashdropDbContext(DbContextOptions<FlashdropDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Product> Products { get; set; } = null!;
    public DbSet<Sale> Sales { get; set; } = null!;
    public DbSet<Reservation> Reservations { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<OutboxMessage> OutboxMessages { get; set; } = null!;
    public DbSet<NotificationLog> NotificationLogs { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductConfiguration).Assembly);
    }
}
