namespace CarWashBooking.Models.Entities;

public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public string? Phone { get; set; }
    public int RoleId { get; set; }
    public int TotalPoints { get; set; }
    public string? LicensePlate { get; set; }
    public DateTime CreatedAt { get; set; }

    public Role Role { get; set; } = null!;
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<Session> Sessions { get; set; } = new List<Session>();
}
