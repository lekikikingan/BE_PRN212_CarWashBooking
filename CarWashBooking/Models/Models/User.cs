namespace CarWashBooking.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Role { get; set; } = "CUSTOMER";
        public int TotalPoints { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}