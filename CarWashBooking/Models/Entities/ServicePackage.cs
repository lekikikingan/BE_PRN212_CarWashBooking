namespace CarWashBooking.Models.Entities;

public class ServicePackage
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int RewardPoints { get; set; }
    public bool IsActive { get; set; }

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
