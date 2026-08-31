namespace Flashdrop.Data.Entities;

/// <summary>
/// Represents a product in the catalog.
/// A product can have zero or more sales over its lifetime.
/// </summary>
public class Product
{
    public Guid Id { get; set; }
    public Guid SellerId { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public decimal BasePrice { get; set; }
    public string? Category { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    // Navigation properties
    public User Seller { get; set; } = null!;
    public ICollection<Sale> Sales { get; set; } = [];
}
