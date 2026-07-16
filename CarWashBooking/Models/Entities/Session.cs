namespace CarWashBooking.Models.Entities;

public class Session
{
    public string SessionId { get; set; } = null!;
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }

    public User User { get; set; } = null!;
}
