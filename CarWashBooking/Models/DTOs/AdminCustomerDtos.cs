namespace CarWashBooking.Models.DTOs;

/// <summary>
/// DTO trả về thông tin chi tiết của một khách hàng cho Admin (US-20 / BR-20).
/// </summary>
public class CustomerDetailDto
{
    /// <summary>Họ và tên đầy đủ.</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>Số điện thoại (có thể null).</summary>
    public string? Phone { get; set; }

    /// <summary>Email đăng nhập.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Biển số xe (có thể null).</summary>
    public string? LicensePlate { get; set; }

    /// <summary>Tổng điểm thưởng hiện có.</summary>
    public int TotalPoints { get; set; }
}
