using Flashdrop.Data.Entities;

namespace Flashdrop.Application.Products.DTOs.Response;

public class ProductForListDto
{
    public Guid Id { get; set; }
    public Guid SellerId { get; set; }
    public required string Name { get; set; }
    public string? ImageUrl { get; set; }
    public decimal BasePrice { get; set; }
    public ProductCategory Category { get; set; }
}
