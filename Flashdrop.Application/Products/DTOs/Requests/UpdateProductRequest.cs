using Flashdrop.Domain.Entities;

namespace Flashdrop.Application.Products.DTOs.Requests;

public class UpdateProductRequest
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public decimal BasePrice { get; set; }
    public ProductCategory Category { get; set; }
}
