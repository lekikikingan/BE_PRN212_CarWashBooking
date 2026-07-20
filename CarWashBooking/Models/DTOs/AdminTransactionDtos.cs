namespace CarWashBooking.Models.DTOs;

/// <summary>
/// DTO trả về kết quả sau khi Admin xác nhận hoàn thành booking (US-23 / BR-23).
/// </summary>
public class CompleteBookingResultDto
{
    /// <summary>Mã booking vừa được cập nhật.</summary>
    public int BookingId { get; set; }

    /// <summary>Trạng thái mới của booking (COMPLETED).</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>Thời điểm hoàn thành.</summary>
    public DateTime? CompletedAt { get; set; }
}
