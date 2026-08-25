using Flashdrop.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flashdrop.Data.Configuration;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.ProductId)
            .IsRequired();

        builder.Property(s => s.TotalStock)
            .IsRequired();

        builder.Property(s => s.AvailableStock)
            .IsRequired();

        builder.Property(s => s.PricePerUnit)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(s => s.MaxUnitsPerUser)
            .IsRequired();

        builder.Property(s => s.StartsAt)
            .IsRequired();

        builder.Property(s => s.EndsAt)
            .IsRequired();

        builder.Property(s => s.Status)
            .HasConversion<string>()
            .IsRequired();

        // Concurrency token — EF Core uses this to detect concurrent modifications
        builder.Property(s => s.RowVersion)
            .IsRowVersion()
            .IsRequired();

        // Composite index for sales list queries
        builder.HasIndex(s => new { s.Status, s.StartsAt })
            .HasDatabaseName("IX_Sale_Status_StartsAt");

        // Index for Redis cache-aside lookups
        builder.HasIndex(s => s.ProductId);

        // Relationships
        builder.HasOne(s => s.Product)
            .WithMany(p => p.Sales)
            .HasForeignKey(s => s.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.Reservations)
            .WithOne(r => r.Sale)
            .HasForeignKey(r => r.SaleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.Orders)
            .WithOne(o => o.Sale)
            .HasForeignKey(o => o.SaleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
