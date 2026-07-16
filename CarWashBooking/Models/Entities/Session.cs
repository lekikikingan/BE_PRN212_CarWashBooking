namespace CarWashBooking.Models.Entities;

/// <summary>
/// Phiên đăng nhập của người dùng.
/// Được tạo khi login thành công, xóa khi logout.
/// SessionId được lưu trong cookie "session_id" trên trình duyệt.
/// </summary>
public class Session
{
    /// <summary>
    /// Chuỗi ngẫu nhiên 64 ký tự dùng làm khóa phiên.
    /// Giá trị này được lưu vào cookie trên trình duyệt của người dùng.
    /// </summary>
    public string SessionId { get; set; } = null!;

    /// <summary>FK trỏ đến người dùng đang đăng nhập.</summary>
    public int UserId { get; set; }

    /// <summary>Thời điểm tạo phiên (UTC).</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Thời điểm hết hạn phiên (UTC).
    /// Sau thời điểm này, phiên không còn hợp lệ và người dùng cần đăng nhập lại.
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    // Navigation property
    public User User { get; set; } = null!;
}
