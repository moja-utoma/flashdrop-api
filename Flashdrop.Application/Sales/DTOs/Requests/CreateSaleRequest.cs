using Flashdrop.Domain.Entities;

namespace Flashdrop.Application.Sales.DTOs.Requests;

public class CreateSaleRequest
{
    public Guid ProductId { get; set; }
    public int TotalStock { get; set; }
    public decimal PricePerUnit { get; set; }
    public int MaxUnitsPerUser { get; set; }
    public DateTimeOffset StartsAt { get; set; }
    public DateTimeOffset EndsAt { get; set; }
}