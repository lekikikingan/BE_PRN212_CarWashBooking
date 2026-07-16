namespace CarWashBooking.Models.Entities;

public class Booking
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public int PackageId { get; set; }
    public DateOnly AppointmentDate { get; set; }
    public int TimeSlotId { get; set; }
    public BookingStatus Status { get; set; }
    public decimal? AmountPaid { get; set; }
    public int PointsUsed { get; set; }
    public int PointsEarned { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    public User Customer { get; set; } = null!;
    public ServicePackage Package { get; set; } = null!;
    public TimeSlot TimeSlot { get; set; } = null!;
}
