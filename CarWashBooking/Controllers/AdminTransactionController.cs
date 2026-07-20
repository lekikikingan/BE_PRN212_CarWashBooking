using CarWashBooking.Models.Common;
using CarWashBooking.Models.DTOs;
using CarWashBooking.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarWashBooking.Controllers;

/// <summary>
/// Controller quản lý giao dịch dành cho Admin (Module MD-06).
/// US-23 (Xác nhận hoàn thành booking).
/// </summary>
[ApiController]
[Route("api/admin/transactions")]
[Authorize(Roles = "Admin")]
public class AdminTransactionController : ControllerBase
{
    private readonly IAdminTransactionService _service;

    public AdminTransactionController(IAdminTransactionService service) => _service = service;

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
