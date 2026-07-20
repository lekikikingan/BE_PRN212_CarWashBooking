using CarWashBooking.Models.Entities;

namespace CarWashBooking.Models.DTOs;

/// <summary>
/// DTO trả về mỗi dòng giao dịch trong danh sách của Admin (US-21 / BR-21, US-22 / BR-22).
/// Chỉ gồm booking có trạng thái PAID hoặc COMPLETED.
/// </summary>
public class TransactionItemDto
{
    /// <summary>Mã booking (giao dịch).</summary>
    public int BookingId { get; set; }

    /// <summary>Tên khách hàng (join từ bảng Users).</summary>
    public string CustomerName { get; set; } = string.Empty;

    /// <summary>Tên gói dịch vụ (join từ bảng Packages).</summary>
    public string PackageName { get; set; } = string.Empty;

    /// <summary>Số tiền khách đã thanh toán (VND).</summary>
    public decimal? AmountPaid { get; set; }

    /// <summary>Số điểm khách đã dùng để giảm giá.</summary>
    public int PointsUsed { get; set; }

    /// <summary>Số điểm khách nhận được sau thanh toán.</summary>
    public int PointsEarned { get; set; }

    /// <summary>Trạng thái booking (PAID hoặc COMPLETED).</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>Thời điểm thanh toán.</summary>
    public DateTime? PaidAt { get; set; }
}

/// <summary>
/// DTO nhận điều kiện lọc danh sách giao dịch từ Admin (US-22 / BR-22).
/// Tất cả tham số đều tùy chọn; kết hợp bằng logic AND.
/// </summary>
public class TransactionFilterDto
{
    /// <summary>
    /// Lọc theo trạng thái booking. Giá trị hợp lệ: PAID, COMPLETED, CANCELLED.
    /// Null = không lọc theo trạng thái.
    /// </summary>
    public BookingStatus? Status { get; set; }

    /// <summary>Lọc theo ngày hẹn từ (bao gồm). Null = không giới hạn cận dưới.</summary>
    public DateOnly? FromDate { get; set; }

    /// <summary>Lọc theo ngày hẹn đến (bao gồm). Null = không giới hạn cận trên.</summary>
    public DateOnly? ToDate { get; set; }
}

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
