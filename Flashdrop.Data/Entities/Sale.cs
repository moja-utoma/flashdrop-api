namespace Flashdrop.Data.Entities;

/// <summary>
/// Represents a flash sale for a product.
/// The hot field is AvailableStock which is decremented under contention.
/// Uses RowVersion for optimistic concurrency control.
/// </summary>
public class Sale
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

    // Navigation properties
    public Product Product { get; set; } = null!;
    public ICollection<Reservation> Reservations { get; set; } = [];
    public ICollection<Order> Orders { get; set; } = [];
}

public enum SaleStatus
{
    Scheduled,
    Live,
    Ended,
    Cancelled
}
