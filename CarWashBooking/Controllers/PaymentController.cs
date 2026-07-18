using System.Security.Claims;
using CarWashBooking.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarWashBooking.Controllers;

[ApiController]
[Route("api/payments")]
[Authorize(Roles = "CUSTOMER")] // Note: standard role names in Roles table seed are "CUSTOMER" and "ADMIN" in uppercase
public class PaymentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    /**
     * // comment by Tài: Lấy tổng điểm thưởng hiện có của customer (MD-05-FE-05)
     * @author Tài
     * @version 1.0
     */
    [HttpGet("reward-points")]
    public async Task<IActionResult> GetRewardPoints()
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _paymentService.GetRewardPointsAsync(userId);
            return Ok(new
            {
                success = true,
                message = "Lấy điểm thưởng thành công",
                data = result
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return StatusCode(401, new
            {
                success = false,
                message = ex.Message,
                errorCode = "UNAUTHORIZED"
            });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                success = false,
                message = ex.Message,
                errorCode = "USER_NOT_FOUND"
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
