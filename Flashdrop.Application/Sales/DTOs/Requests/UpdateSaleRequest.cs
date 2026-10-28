using Flashdrop.Domain.Entities;

namespace Flashdrop.Application.Sales.DTOs.Requests;

public class UpdateSaleRequest
{
    public int TotalStock { get; set; }
    public decimal PricePerUnit { get; set; }
    public int MaxUnitsPerUser { get; set; }
    public DateTimeOffset StartsAt { get; set; }
    public DateTimeOffset EndsAt { get; set; }
    public byte[] RowVersion { get; set; } = null!;
}