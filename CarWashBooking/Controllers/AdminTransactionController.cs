using CarWashBooking.Models.Common;
using CarWashBooking.Models.DTOs;
using CarWashBooking.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarWashBooking.Controllers;

/// <summary>
/// Controller quản lý giao dịch dành cho Admin (Module MD-06).
/// US-21 (Xem danh sách giao dịch).
/// </summary>
[ApiController]
[Route("api/admin/transactions")]
[Authorize(Roles = "Admin")]
public class AdminTransactionController : ControllerBase
{
    private readonly IAdminTransactionService _service;

    public AdminTransactionController(IAdminTransactionService service) => _service = service;

    /// <summary>
    /// API Xem danh sách giao dịch (US-21 / BR-21).
    /// Endpoint: GET /api/admin/transactions
    /// </summary>
    /// <returns>
    /// ApiResponse&lt;List&lt;TransactionItemDto&gt;&gt;. Chỉ gồm booking PAID/COMPLETED, sắp xếp PaidAt giảm dần.
    /// - 200 OK: trả về danh sách (có thể rỗng).
    /// </returns>
    [HttpGet]
    public async Task<IActionResult> GetTransactions()
    {
        var transactions = await _service.GetTransactionsAsync();
        return Ok(ApiResponse<List<TransactionItemDto>>.Ok(transactions, "Lấy danh sách giao dịch thành công"));
    }
}
