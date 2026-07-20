namespace CarWashBooking.Models.DTOs;

/// <summary>
/// DTO trả về mỗi dòng giao dịch trong danh sách của Admin (US-21 / BR-21).
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
