using Flashdrop.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flashdrop.Data.Configuration;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);

        builder.Property(o => o.ReservationId)
            .IsRequired();

        builder.Property(o => o.UserId)
            .IsRequired();

        builder.Property(o => o.SaleId)
            .IsRequired();

        builder.Property(o => o.Quantity)
            .IsRequired();

        builder.Property(o => o.TotalPrice)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(o => o.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(o => o.IdempotencyKey)
            .IsRequired();

        builder.Property(o => o.CreatedAt)
            .IsRequired();

        // Unique index on IdempotencyKey for idempotent retries
        builder.HasIndex(o => o.IdempotencyKey)
            .IsUnique()
            .HasDatabaseName("IX_Order_IdempotencyKey");

        // Indexes for common queries
        builder.HasIndex(o => o.UserId);
        builder.HasIndex(o => o.SaleId);
        builder.HasIndex(o => o.CreatedAt);

        // Relationships
        builder.HasOne(o => o.Reservation)
            .WithOne(r => r.Order)
            .HasForeignKey<Order>(o => o.ReservationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.User)
            .WithMany()
            .HasForeignKey(o => o.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(o => o.Sale)
            .WithMany(s => s.Orders)
            .HasForeignKey(o => o.SaleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(o => o.NotificationLogs)
            .WithOne(nl => nl.Order)
            .HasForeignKey(nl => nl.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(o => o.OutboxMessages)
            .WithOne(om => om.Order)
            .HasForeignKey(om => om.OrderId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
