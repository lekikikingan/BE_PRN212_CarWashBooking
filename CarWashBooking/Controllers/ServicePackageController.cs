using CarWashBooking.Models.Common;
using CarWashBooking.Models.DTOs;
using CarWashBooking.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarWashBooking.Controllers;

/// <summary>
/// Controller xử lý các yêu cầu liên quan đến Gói dịch vụ rửa xe cho Admin.
/// </summary>
[ApiController]
[Route("api/service-packages")]
public class ServicePackageController : ControllerBase
{
    private readonly IServicePackageService _servicePackageService;

    /// <summary>
    /// Khởi tạo ServicePackageController với IServicePackageService được Inject qua Constructor.
    /// </summary>
    /// <param name="servicePackageService">IServicePackageService - NGUỒN: Inject qua DI Container.</param>
    public ServicePackageController(IServicePackageService servicePackageService)
    {
        _servicePackageService = servicePackageService;
    }

    /// <summary>
    /// Tạo một gói dịch vụ mới (Dành cho Admin - US-05).
    /// Endpoint: POST /api/service-packages
    /// </summary>
    /// <param name="requestDto">
    /// CreatePackageRequestDto - NGUỒN: FE truyền lên qua Request Body - Chứa Name, Description, Price, RewardPoints.
    /// </param>
    /// <returns>
    /// ApiResponse&lt;PackageResponseDto&gt;. Data gồm: Id, Name, Description, Price, RewardPoints, IsActive.
    /// Trả 200 OK nếu tạo thành công (AC1).
    /// Trả 400 Bad Request nếu:
    /// - Tên gói bị bỏ trống hoặc chỉ chứa khoảng trắng: "Package name is required" (AC4)
    /// - Giá gói &lt;= 0: "Package price must be greater than 0" (AC2)
    /// - Điểm thưởng &lt; 0: "Reward points must be a non-negative integer" (AC3)
    /// </returns>
    [HttpPost]
    public async Task<IActionResult> CreatePackage([FromBody] CreatePackageRequestDto requestDto)
    {
        // AC4 — Tên gói bị bỏ trống hoặc chỉ nhập khoảng trắng
        if (string.IsNullOrWhiteSpace(requestDto.Name))
        {
            return BadRequest(ApiResponse<PackageResponseDto>.Fail(400, "Package name is required"));
        }

        // AC2 — Giá không hợp lệ (<= 0)
        if (requestDto.Price <= 0)
        {
            return BadRequest(ApiResponse<PackageResponseDto>.Fail(400, "Package price must be greater than 0"));
        }

        // AC3 — Điểm thưởng không hợp lệ (< 0)
        if (requestDto.RewardPoints < 0)
        {
            return BadRequest(ApiResponse<PackageResponseDto>.Fail(400, "Reward points must be a non-negative integer"));
        }

        // AC1 — Tạo gói thành công
        var result = await _servicePackageService.CreatePackageAsync(requestDto);
        return Ok(ApiResponse<PackageResponseDto>.Ok(result, "Tạo gói dịch vụ thành công."));
    }
}
