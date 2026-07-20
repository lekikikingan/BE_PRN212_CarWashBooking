using CarWashBooking.Models.Common;
using CarWashBooking.Models.DTOs;
using CarWashBooking.Models.Entities;
using CarWashBooking.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarWashBooking.Controllers;

/// <summary>
/// Controller quản lý giao dịch dành cho Admin (Module MD-06).
/// US-21 (Xem danh sách giao dịch), US-22 (Lọc giao dịch theo trạng thái/ngày), US-23 (Xác nhận hoàn thành booking).
/// </summary>
[ApiController]
[Route("api/admin/transactions")]
[Authorize(Roles = "Admin")]
public class AdminTransactionController : ControllerBase
{
    private readonly IAdminTransactionService _service;

    public AdminTransactionController(IAdminTransactionService service) => _service = service;

    /// <summary>
    /// API Xem và lọc danh sách giao dịch (US-21 / BR-21, US-22 / BR-22).
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

    /// <summary>
    /// API Xác nhận hoàn thành một booking đã thanh toán (US-23 / BR-23).
    /// Endpoint: PATCH /api/admin/transactions/{id}/complete
    /// </summary>
    /// <param name="id">Int - NGUỒN: Route Parameter - Mã booking cần xác nhận hoàn thành.</param>
    /// <returns>
    /// ApiResponse&lt;CompleteBookingResultDto&gt;.
    /// - 200 OK: cập nhật thành công (PAID → COMPLETED, ghi CompletedAt).
    /// - 404 Not Found: booking không tồn tại ("Không tìm thấy booking").
    /// - 409 Conflict: booking không ở trạng thái PAID ("Chỉ có thể xác nhận hoàn thành cho booking đã thanh toán").
    /// </returns>
    [HttpPatch("{id:int}/complete")]
    public async Task<IActionResult> CompleteBooking(int id)
    {
        try
        {
            var result = await _service.CompleteBookingAsync(id);

            if (result == null)
            {
                return NotFound(ApiResponse<CompleteBookingResultDto>.Fail(404, "Không tìm thấy booking"));
            }

            return Ok(ApiResponse<CompleteBookingResultDto>.Ok(result, "Xác nhận hoàn thành booking thành công"));
        }
        catch (InvalidOperationException ex)
        {
            // BR-23 EXCEPTION 1 — Booking không ở trạng thái PAID
            return Conflict(ApiResponse<CompleteBookingResultDto>.Fail(409, ex.Message));
        }
    }
}
