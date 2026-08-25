namespace Flashdrop.Data.Entities;

/// <summary>
/// Represents a finalized purchase created from a paid reservation.
/// Uses IdempotencyKey to make retrying "confirm payment" safe.
/// </summary>
public class Order
{
    public Guid Id { get; set; }
    public Guid ReservationId { get; set; }
    public Guid UserId { get; set; }
    public Guid SaleId { get; set; }
    public int Quantity { get; set; }
    public decimal TotalPrice { get; set; }
    public OrderStatus Status { get; set; }
    public Guid IdempotencyKey { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    // Navigation properties
    public Reservation Reservation { get; set; } = null!;
    public User User { get; set; } = null!;
    public Sale Sale { get; set; } = null!;
    public ICollection<NotificationLog> NotificationLogs { get; set; } = [];
    public ICollection<OutboxMessage> OutboxMessages { get; set; } = [];
}

public enum OrderStatus
{
    Created,
    ConfirmationSent,
    Failed
}
