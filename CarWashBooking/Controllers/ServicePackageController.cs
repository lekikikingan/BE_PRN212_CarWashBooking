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

    public ServicePackageController(IServicePackageService servicePackageService)
    {
        _servicePackageService = servicePackageService;
    }

    /// <summary>
    /// Tạo một gói dịch vụ mới (Dành cho Admin - US-05).
    /// Endpoint: POST /api/service-packages
    /// </summary>
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

    /// <summary>
    /// Lấy toàn bộ danh sách gói dịch vụ kể cả gói đang ẩn cho Admin (US-06).
    /// Endpoint: GET /api/service-packages
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllPackages()
    {
        var list = await _servicePackageService.GetAllPackagesAsync();

        // AC2 — Không có gói nào trong CSDL
        if (list.Count == 0)
        {
            return Ok(ApiResponse<List<PackageResponseDto>>.Ok(list, "No packages found"));
        }

        // AC1 — Hiển thị toàn bộ danh sách gói
        return Ok(ApiResponse<List<PackageResponseDto>>.Ok(list, $"Lấy thành công {list.Count} gói dịch vụ."));
    }

    /// <summary>
    /// API Lấy chi tiết một gói dịch vụ theo ID.
    /// Endpoint: GET /api/service-packages/{id}
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetPackageById(int id)
    {
        var package = await _servicePackageService.GetPackageByIdAsync(id);

        if (package == null)
        {
            return NotFound(ApiResponse<PackageResponseDto>.Fail(404, "Package not found"));
        }

        return Ok(ApiResponse<PackageResponseDto>.Ok(package, "Lấy chi tiết gói dịch vụ thành công."));
    }

    /// <summary>
    /// API Chỉnh sửa gói dịch vụ theo ID (US-07).
    /// Endpoint: PUT /api/service-packages/{id}
    /// </summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdatePackage(int id, [FromBody] UpdatePackageRequestDto requestDto)
    {
        // AC4 — Điểm thưởng không hợp lệ (< 0)
        if (requestDto.RewardPoints < 0)
        {
            return BadRequest(ApiResponse<PackageResponseDto>.Fail(400, "Reward points must be a non-negative integer"));
        }

        // AC3 — Giá không hợp lệ (<= 0)
        if (requestDto.Price <= 0)
        {
            return BadRequest(ApiResponse<PackageResponseDto>.Fail(400, "Package price must be greater than 0"));
        }

        // Validate tên gói rỗng
        if (string.IsNullOrWhiteSpace(requestDto.Name))
        {
            return BadRequest(ApiResponse<PackageResponseDto>.Fail(400, "Package name is required"));
        }

        var result = await _servicePackageService.UpdatePackageAsync(id, requestDto);

        // AC2 — Gói không tồn tại trong CSDL
        if (result == null)
        {
            return NotFound(ApiResponse<PackageResponseDto>.Fail(404, "Package not found"));
        }

        // AC1 — Sửa thành công
        return Ok(ApiResponse<PackageResponseDto>.Ok(result, "Cập nhật gói dịch vụ thành công."));
    }
}
