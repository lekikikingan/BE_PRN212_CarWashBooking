namespace CarWashBooking.Models.Entities;

/// <summary>
/// Vai trò người dùng trong hệ thống.
/// Chỉ có 2 giá trị hợp lệ được seed sẵn: "CUSTOMER" và "ADMIN".
/// </summary>
public class Role
{
    public int Id { get; set; }

    /// <summary>Tên vai trò: "CUSTOMER" hoặc "ADMIN".</summary>
    public string Name { get; set; } = null!;

    // Navigation property — danh sách user thuộc vai trò này
    public ICollection<User> Users { get; set; } = new List<User>();
}
