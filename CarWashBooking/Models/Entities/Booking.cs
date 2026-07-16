namespace CarWashBooking.Models.Entities;

/// <summary>
/// Lịch đặt rửa xe của khách hàng.
/// Một booking gắn với một khách hàng, một gói dịch vụ và một khung giờ cụ thể.
/// </summary>
public class Booking
{
    public int Id { get; set; }

    /// <summary>FK trỏ đến khách hàng đặt lịch.</summary>
    public int CustomerId { get; set; }

    /// <summary>FK trỏ đến gói dịch vụ được chọn.</summary>
    public int PackageId { get; set; }

    /// <summary>Ngày hẹn rửa xe.</summary>
    public DateOnly AppointmentDate { get; set; }

    /// <summary>FK trỏ đến khung giờ được chọn trong ngày.</summary>
    public int TimeSlotId { get; set; }

    /// <summary>Trạng thái hiện tại của booking. Xem enum BookingStatus để biết luồng chuyển trạng thái.</summary>
    public BookingStatus Status { get; set; }

    /// <summary>
    /// Số tiền thực tế khách đã thanh toán (VND).
    /// NULL khi chưa thanh toán (Status = PendingPayment).
    /// Có thể thấp hơn giá gói nếu khách dùng điểm thưởng để giảm giá.
    /// </summary>
    public decimal? AmountPaid { get; set; }

    /// <summary>
    /// Số điểm thưởng khách dùng để giảm giá cho booking này.
    /// Mặc định là 0 (không dùng điểm). Không được âm.
    /// </summary>
    public int PointsUsed { get; set; }

    /// <summary>
    /// Số điểm thưởng khách nhận được sau khi thanh toán booking này.
    /// Bằng với RewardPoints của gói dịch vụ tại thời điểm đặt.
    /// </summary>
    public int PointsEarned { get; set; }

    /// <summary>Thời điểm tạo booking (UTC). Tự động gán bởi database.</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>Thời điểm thanh toán thành công (UTC). NULL nếu chưa thanh toán.</summary>
    public DateTime? PaidAt { get; set; }

    /// <summary>Thời điểm hủy booking (UTC). NULL nếu chưa hủy.</summary>
    public DateTime? CancelledAt { get; set; }

    /// <summary>Thời điểm hoàn thành dịch vụ (UTC). NULL nếu chưa hoàn thành.</summary>
    public DateTime? CompletedAt { get; set; }

    // Navigation properties
    public User Customer { get; set; } = null!;
    public ServicePackage Package { get; set; } = null!;
    public TimeSlot TimeSlot { get; set; } = null!;
}
