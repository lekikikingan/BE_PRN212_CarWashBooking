namespace CarWashBooking.Models.Entities;

/// <summary>
/// Gói dịch vụ rửa xe (map xuống bảng "Packages" trong DB).
/// Admin có thể tạo, chỉnh sửa và ẩn/hiện gói dịch vụ.
/// </summary>
public class ServicePackage
{
    public int Id { get; set; }

    /// <summary>Tên gói dịch vụ, tối đa 100 ký tự. Ví dụ: "Basic", "Premium".</summary>
    public string Name { get; set; } = null!;

    /// <summary>Mô tả chi tiết dịch vụ bao gồm những gì. Có thể để trống.</summary>
    public string? Description { get; set; }

    /// <summary>Giá dịch vụ (VND), phải lớn hơn 0.</summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Số điểm thưởng khách nhận được sau khi thanh toán gói này.
    /// Điểm được cộng vào Users.TotalPoints.
    /// </summary>
    public int RewardPoints { get; set; }

    /// <summary>
    /// Trạng thái hiển thị của gói dịch vụ.
    /// true = đang hoạt động, khách hàng có thể đặt.
    /// false = đã ẩn (soft delete), không hiển thị cho khách nhưng dữ liệu vẫn còn trong DB.
    /// </summary>
    public bool IsActive { get; set; }

    // Navigation property
    public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
}
