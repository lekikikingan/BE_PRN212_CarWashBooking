namespace CarWashBooking.Models.Entities;

public class TimeSlot
{
    public int Id { get; set; }
    public TimeOnly SlotTime { get; set; }

    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
