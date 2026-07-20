using CarWashBooking.Models.DTOs;

namespace CarWashBooking.Services;

/// <summary>
/// Quản lý khách hàng từ phía Admin: xem danh sách và chi tiết.
/// Chỉ trả về user có role = CUSTOMER, không bao gồm ADMIN.
/// US-19 (Customer List), US-20 (Customer Detail).
/// </summary>
public interface IAdminCustomerService
{
    /// <summary>
    /// Lấy danh sách toàn bộ khách hàng (role = CUSTOMER), sắp xếp theo ngày tạo giảm dần (US-19 / BR-19).
    /// </summary>
    /// <returns>Danh sách CustomerListItemDto. Trả về danh sách rỗng nếu không có khách hàng nào.</returns>
    Task<List<CustomerListItemDto>> GetCustomersAsync();
}
