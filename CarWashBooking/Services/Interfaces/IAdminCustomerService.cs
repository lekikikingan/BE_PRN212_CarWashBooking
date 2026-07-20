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
    /// Lấy thông tin chi tiết một khách hàng theo id (US-20 / BR-20).
    /// </summary>
    /// <param name="customerId">Mã khách hàng cần xem chi tiết.</param>
    /// <returns>
    /// CustomerDetailDto nếu tìm thấy user có id tương ứng và role = CUSTOMER;
    /// null nếu không tồn tại hoặc user không phải khách hàng.
    /// </returns>
    Task<CustomerDetailDto?> GetCustomerDetailAsync(int customerId);
}
