namespace Flashdrop.Data.Entities;

/// <summary>
/// Represents a user in the system with either Customer or Admin role.
/// </summary>
public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public UserRole Role { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public bool IsActive { get; set; }

    // Navigation properties
    public ICollection<Product> SoldProducts { get; set; } = [];
    public ICollection<Reservation> Reservations { get; set; } = [];
}

public enum UserRole
{
    Customer,
    Admin
}
