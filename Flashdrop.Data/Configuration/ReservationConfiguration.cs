using Flashdrop.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flashdrop.Data.Configuration;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.SaleId)
            .IsRequired();

        builder.Property(r => r.UserId)
            .IsRequired();

        builder.Property(r => r.Quantity)
            .IsRequired();

        builder.Property(r => r.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(r => r.CreatedAt)
            .IsRequired();

        builder.Property(r => r.ExpiresAt)
            .IsRequired();

        builder.Property(r => r.ConfirmedAt)
            .IsRequired(false);

        // Composite index to enforce per-user purchase cap efficiently
        builder.HasIndex(r => new { r.SaleId, r.UserId })
            .HasDatabaseName("IX_Reservation_Sale_User");

        // Index for background expiry sweep service
        builder.HasIndex(r => new { r.Status, r.ExpiresAt })
            .HasDatabaseName("IX_Reservation_Status_ExpiresAt");

        // Relationships
        builder.HasOne(r => r.Sale)
            .WithMany(s => s.Reservations)
            .HasForeignKey(r => r.SaleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.User)
            .WithMany(u => u.Reservations)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Order)
            .WithOne(o => o.Reservation)
            .HasForeignKey<Order>(o => o.ReservationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
