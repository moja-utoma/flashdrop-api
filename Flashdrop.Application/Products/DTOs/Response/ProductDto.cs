using Flashdrop.Domain.Entities;

namespace Flashdrop.Application.Products.DTOs.Response;

public class ProductDto
{
    public Guid Id { get; set; }
    public Guid SellerId { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public string? ImageUrl { get; set; }
    public decimal BasePrice { get; set; }
    public ProductCategory Category { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
