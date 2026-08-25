using Flashdrop.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flashdrop.Data.Configuration;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(p => p.ImageUrl)
            .HasMaxLength(500);

        builder.Property(p => p.BasePrice)
            .HasPrecision(10, 2)
            .IsRequired();

        builder.Property(p => p.Category)
            .HasMaxLength(100);

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.SellerId)
            .IsRequired();

        // Index for querying products by seller
        builder.HasIndex(p => p.SellerId);

        // Index for filtering by category
        builder.HasIndex(p => p.Category);

        // Relationships
        builder.HasOne(p => p.Seller)
            .WithMany(u => u.SoldProducts)
            .HasForeignKey(p => p.SellerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(p => p.Sales)
            .WithOne(s => s.Product)
            .HasForeignKey(s => s.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
