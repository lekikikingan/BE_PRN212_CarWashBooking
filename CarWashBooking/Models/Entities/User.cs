namespace CarWashBooking.Models.Entities;

/// <summary>
/// Người dùng hệ thống — gồm cả khách hàng (CUSTOMER) và quản trị viên (ADMIN).
/// </summary>
public class User
{
    public int Id { get; set; }

    /// <summary>Họ và tên đầy đủ, tối đa 100 ký tự.</summary>
    public string FullName { get; set; } = null!;

    /// <summary>Email dùng để đăng nhập, phải là duy nhất trong hệ thống.</summary>
    public string Email { get; set; } = null!;

    /// <summary>
    /// Mật khẩu đã được hash bằng SHA256.
    /// Không bao giờ lưu mật khẩu dạng plain text.
    /// </summary>
    public string PasswordHash { get; set; } = null!;

    /// <summary>Số điện thoại 10 chữ số, bắt đầu bằng 0. Có thể để trống.</summary>
    public string? Phone { get; set; }

    /// <summary>FK trỏ đến bảng Roles (CUSTOMER = 1, ADMIN = 2).</summary>
    public int RoleId { get; set; }

    /// <summary>
    /// Tổng điểm thưởng tích lũy. Không được âm.
    /// Tăng khi thanh toán booking, giảm khi dùng điểm để giảm giá.
    /// </summary>
    public int TotalPoints { get; set; }

    /// <summary>Biển số xe của khách hàng, ví dụ "51A-12345". Có thể để trống.</summary>
    public string? LicensePlate { get; set; }

    /// <summary>Thời điểm tạo tài khoản (UTC). Tự động gán bởi database.</summary>
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public Role Role { get; set; } = null!;
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    public ICollection<Session> Sessions { get; set; } = new List<Session>();
}
