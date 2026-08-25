namespace Flashdrop.Data.Entities;

/// <summary>
/// Provides an audit trail for notification attempts.
/// Useful for demonstrating DLQ behavior and debugging notification failures.
/// </summary>
public class NotificationLog
{
    public Guid Id { get; set; }
    public Guid OrderId { get; set; }
    public NotificationChannel Channel { get; set; }
    public NotificationStatus Status { get; set; }
    public int Attempts { get; set; }
    public DateTimeOffset LastAttemptAt { get; set; }

    // Navigation property
    public Order Order { get; set; } = null!;
}

public enum NotificationChannel
{
    Email
}

public enum NotificationStatus
{
    Sent,
    Failed,
    DeadLettered
}
