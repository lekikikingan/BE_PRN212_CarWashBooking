using System.Security.Claims;
using CarWashBooking.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarWashBooking.Controllers;

[ApiController]
[Route("api/bookings")]
[Authorize(Roles = "CUSTOMER")]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    /**
     * // comment by Tài: Hủy lịch đặt xe
     * @author Tài
     * @version 1.0
     */
    [HttpPost("{bookingId}/cancel")]
    public async Task<IActionResult> CancelBooking(int bookingId)
    {
        try
        {
            var userId = GetCurrentUserId();
            await _bookingService.CancelBookingAsync(userId, bookingId);
            return Ok(new
            {
                success = true,
                message = "Booking cancelled successfully"
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            // Trả về HTTP 403 Forbidden nếu không phải chủ sở hữu booking
            return StatusCode(403, new
            {
                success = false,
                message = ex.Message,
                errorCode = "FORBIDDEN"
            });
        }
        catch (KeyNotFoundException ex)
        {
            // Trả về HTTP 404 Not Found nếu không tìm thấy booking
            return NotFound(new
            {
                success = false,
                message = ex.Message,
                errorCode = "NOT_FOUND"
            });
        }
        catch (InvalidOperationException ex)
        {
            // Trả về HTTP 409 Conflict nếu booking không ở trạng thái PendingPayment
            return StatusCode(409, new
            {
                success = false,
                message = ex.Message,
                errorCode = "CONFLICT"
            });
        }
        catch (Exception)
        {
            return StatusCode(500, new
            {
                success = false,
                message = "Đã xảy ra lỗi hệ thống.",
                errorCode = "INTERNAL_SERVER_ERROR"
            });
        }
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Phiên đăng nhập không hợp lệ hoặc đã hết hạn.");
        }
        return userId;
    }
}
