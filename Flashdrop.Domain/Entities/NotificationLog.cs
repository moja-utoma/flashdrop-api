namespace Flashdrop.Data.Entities;

/// <summary>
/// Represents an event ready to be published.
/// Ensures that "decrement stock + create order" and "publish event to message broker"
/// happen atomically from the app's point of view.
/// Prevents message loss if the DB transaction succeeds but the broker publish fails.
/// </summary>
public class OutboxMessage
{
    public Guid Id { get; set; }
    public string Type { get; set; } = null!;
    public string Payload { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? ProcessedAt { get; set; }
    public int RetryCount { get; set; }
    public Guid? OrderId { get; set; }

    // Navigation property
    public Order? Order { get; set; }
}
