namespace CarWashBooking.Models.Entities;

/// <summary>
/// Khung giờ cố định trong ngày để đặt lịch rửa xe.
/// Hệ thống có đúng 24 khung giờ, cách nhau 30 phút: 08:00, 08:30, ..., 19:30.
/// Dữ liệu này được seed sẵn và không thay đổi trong runtime.
/// </summary>
public class TimeSlot
{
    public int Id { get; set; }

    /// <summary>Giờ bắt đầu của khung giờ. Ví dụ: 08:00, 08:30, ..., 19:30.</summary>
    public TimeOnly SlotTime { get; set; }

    // Navigation property
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
