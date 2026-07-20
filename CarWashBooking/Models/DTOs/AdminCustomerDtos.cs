namespace CarWashBooking.Models.DTOs;

/// <summary>
/// DTO trả về mỗi dòng trong danh sách khách hàng cho Admin (US-19 / BR-19).
/// </summary>
public class CustomerListItemDto
{
    /// <summary>Mã định danh khách hàng.</summary>
    public int Id { get; set; }

    /// <summary>Email đăng nhập của khách hàng.</summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>Họ và tên đầy đủ.</summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>Số điện thoại (có thể null).</summary>
    public string? Phone { get; set; }

    /// <summary>Tổng điểm thưởng hiện có.</summary>
    public int TotalPoints { get; set; }
}
