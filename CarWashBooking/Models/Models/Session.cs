namespace CarWashBooking.Models
{
    public class Session
    {
        public string Id { get; set; } = string.Empty;
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}