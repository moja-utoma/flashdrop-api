namespace Flashdrop.Data.Entities;

/// <summary>
/// Represents a hold on inventory during checkout.
/// This is the audit trail of every attempt to claim stock.
/// Expired/cancelled rows are kept for analytics purposes.
/// </summary>
public class Reservation
{
    public Guid Id { get; set; }
    public Guid SaleId { get; set; }
    public Guid UserId { get; set; }
    public int Quantity { get; set; }
    public ReservationStatus Status { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset? ConfirmedAt { get; set; }

    // Navigation properties
    public Sale Sale { get; set; } = null!;
    public User User { get; set; } = null!;
    public Order? Order { get; set; }
}

public enum ReservationStatus
{
    Pending,
    Paid,
    Expired,
    Cancelled
}
