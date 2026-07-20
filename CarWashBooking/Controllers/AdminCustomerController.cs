using CarWashBooking.Models.Common;
using CarWashBooking.Models.DTOs;
using CarWashBooking.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarWashBooking.Controllers;

/// <summary>
/// Controller quản lý khách hàng dành cho Admin (Module MD-06).
/// US-19 (Xem danh sách Customer).
/// </summary>
[ApiController]
[Route("api/admin/customers")]
[Authorize(Roles = "Admin")]
public class AdminCustomerController : ControllerBase
{
    private readonly IAdminCustomerService _service;

    public AdminCustomerController(IAdminCustomerService service) => _service = service;

    /// <summary>
    /// API Xem danh sách toàn bộ khách hàng (US-19 / BR-19).
    /// Endpoint: GET /api/admin/customers
    /// </summary>
    /// <returns>
    /// ApiResponse&lt;List&lt;CustomerListItemDto&gt;&gt;. Data gồm danh sách user có role = CUSTOMER,
    /// sắp xếp theo ngày tạo giảm dần. Không bao gồm tài khoản Admin.
    /// - 200 OK: trả về danh sách (có thể rỗng).
    /// </returns>
    [HttpGet]
    public async Task<IActionResult> GetCustomers()
    {
        var customers = await _service.GetCustomersAsync();
        return Ok(ApiResponse<List<CustomerListItemDto>>.Ok(customers, "Lấy danh sách khách hàng thành công"));
    }
}
