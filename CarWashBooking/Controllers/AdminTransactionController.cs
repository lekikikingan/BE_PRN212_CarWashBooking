using CarWashBooking.Models.Common;
using CarWashBooking.Models.DTOs;
using CarWashBooking.Models.Entities;
using CarWashBooking.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarWashBooking.Controllers;

/// <summary>
/// Controller quản lý giao dịch dành cho Admin (Module MD-06).
/// US-22 (Lọc giao dịch theo trạng thái/ngày).
/// </summary>
[ApiController]
[Route("api/admin/transactions")]
[Authorize(Roles = "Admin")]
public class AdminTransactionController : ControllerBase
{
    private readonly IAdminTransactionService _service;

    public AdminTransactionController(IAdminTransactionService service) => _service = service;

    /// <summary>
    /// API Xem và lọc danh sách giao dịch (US-22 / BR-22).
    /// Endpoint: GET /api/admin/transactions?status=&amp;fromDate=&amp;toDate=
    /// </summary>
    /// <param name="status">
    /// String (tùy chọn) - NGUỒN: Query Parameter - Trạng thái cần lọc: PAID, COMPLETED, CANCELLED.
    /// </param>
    /// <param name="fromDate">DateOnly (tùy chọn) - NGUỒN: Query Parameter - Ngày hẹn từ (yyyy-MM-dd).</param>
    /// <param name="toDate">DateOnly (tùy chọn) - NGUỒN: Query Parameter - Ngày hẹn đến (yyyy-MM-dd).</param>
    /// <returns>
    /// ApiResponse&lt;List&lt;TransactionItemDto&gt;&gt;. Chỉ gồm booking PAID/COMPLETED thỏa điều kiện lọc.
    /// - 200 OK: trả về danh sách; nếu rỗng kèm message "Không có giao dịch phù hợp".
    /// - 400 Bad Request: fromDate &gt; toDate ("Khoảng ngày không hợp lệ"), hoặc status không hợp lệ.
    /// </returns>
    [HttpGet]
    public async Task<IActionResult> GetTransactions(
        [FromQuery] string? status,
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate)
    {
        var filter = new TransactionFilterDto
        {
            FromDate = fromDate,
            ToDate = toDate
        };

        // BR-22 — Validate status filter (nếu có)
        if (!string.IsNullOrWhiteSpace(status))
        {
            if (!Enum.TryParse<BookingStatus>(status, ignoreCase: true, out var parsedStatus)
                || (parsedStatus != BookingStatus.Paid
                    && parsedStatus != BookingStatus.Completed
                    && parsedStatus != BookingStatus.Cancelled))
            {
                return BadRequest(ApiResponse<List<TransactionItemDto>>.Fail(
                    400, "Trạng thái lọc không hợp lệ"));
            }

            filter.Status = parsedStatus;
        }

        // BR-22 EXCEPTION — Khoảng ngày không hợp lệ
        if (fromDate.HasValue && toDate.HasValue && fromDate.Value > toDate.Value)
        {
            return BadRequest(ApiResponse<List<TransactionItemDto>>.Fail(400, "Khoảng ngày không hợp lệ"));
        }

        var transactions = await _service.GetTransactionsAsync(filter);

        if (transactions.Count == 0)
        {
            return Ok(ApiResponse<List<TransactionItemDto>>.Ok(transactions, "Không có giao dịch phù hợp"));
        }

        return Ok(ApiResponse<List<TransactionItemDto>>.Ok(transactions, "Lấy danh sách giao dịch thành công"));
    }
}
