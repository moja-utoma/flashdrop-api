using Flashdrop.Domain.Entities;

namespace Flashdrop.Application.Sales.DTOs.Responses;

public class SaleForListDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int TotalStock { get; set; }
    public int AvailableStock { get; set; }
    public decimal PricePerUnit { get; set; }
    public DateTimeOffset StartsAt { get; set; }
    public SaleStatus Status { get; set; }
}