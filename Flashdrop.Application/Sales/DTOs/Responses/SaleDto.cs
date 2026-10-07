using Flashdrop.Domain.Entities;

namespace Flashdrop.Application.Sales.DTOs.Responses;

public class SaleDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int TotalStock { get; set; }
    public int AvailableStock { get; set; }
    public decimal PricePerUnit { get; set; }
    public int MaxUnitsPerUser { get; set; }
    public DateTimeOffset StartsAt { get; set; }
    public DateTimeOffset EndsAt { get; set; }
    public SaleStatus Status { get; set; }
    public byte[] RowVersion { get; set; } = null!;
}