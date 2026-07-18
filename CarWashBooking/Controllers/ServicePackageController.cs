using CarWashBooking.Models.Common;
using CarWashBooking.Models.DTOs;
using CarWashBooking.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarWashBooking.Controllers;

/// <summary>
/// Controller xử lý các yêu cầu liên quan đến Gói dịch vụ rửa xe cho Admin.
/// - Người thực hiện: Được
/// - Ngày tạo: 18/07/2026
/// </summary>
[ApiController]
[Route("api/service-packages")]
public class ServicePackageController : ControllerBase
{
    private readonly IServicePackageService _servicePackageService;

    /// <summary>
    /// Khởi tạo ServicePackageController với IServicePackageService được Inject qua Constructor.
    /// - Người thực hiện: Được
    /// </summary>
    /// <param name="servicePackageService">IServicePackageService - NGUỒN: Inject qua DI Container.</param>
    public ServicePackageController(IServicePackageService servicePackageService)
    {
        _servicePackageService = servicePackageService;
    }

    /// <summary>
    /// API Tạo một gói dịch vụ mới (Dành cho Admin - US-05).
    /// Endpoint: POST /api/service-packages
    /// - Người thực hiện: Được
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

    /// <summary>
    /// API Lấy toàn bộ danh sách gói dịch vụ kể cả gói đang ẩn cho Admin (US-06).
    /// Endpoint: GET /api/service-packages
    /// - Người thực hiện: Được
    /// </summary>
    /// <returns>
    /// ApiResponse&lt;List&lt;PackageResponseDto&gt;&gt;. Data gồm mảng danh sách toàn bộ các gói dịch vụ.
    /// - 200 OK: Trả về danh sách các gói dịch vụ (AC1).
    /// - 200 OK (Nếu CSDL rỗng): Trả về mảng rỗng kèm message "No packages found" (AC2).
    /// </returns>
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
    /// - Người thực hiện: Được
    /// </summary>
    /// <param name="id">
    /// Int - NGUỒN: FE truyền qua Route Parameter (/api/service-packages/{id}) - Mã ID gói dịch vụ cần xem.
    /// </param>
    /// <returns>
    /// ApiResponse&lt;PackageResponseDto&gt;. Data gồm: Id, Name, Description, Price, RewardPoints, IsActive.
    /// - 200 OK: Lấy thông tin thành công.
    /// - 404 Not Found: Nếu không tìm thấy id ("Package not found").
    /// </returns>
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
    /// - Người thực hiện: Được
    /// </summary>
    /// <param name="id">
    /// Int - NGUỒN: FE truyền qua Route Parameter (/api/service-packages/{id}) - Mã ID gói dịch vụ cần sửa.
    /// </param>
    /// <param name="requestDto">
    /// UpdatePackageRequestDto - NGUỒN: FE truyền qua Request Body - Chứa Name, Description, Price, RewardPoints mới.
    /// </param>
    /// <returns>
    /// ApiResponse&lt;PackageResponseDto&gt;. Data gồm thông tin gói vừa cập nhật.
    /// - 200 OK: Cập nhật thành công (AC1).
    /// - 404 Not Found: Nếu không tìm thấy package_id ("Package not found") (AC2).
    /// - 400 Bad Request nếu Price &lt;= 0 ("Package price must be greater than 0") (AC3), 
    ///   hoặc RewardPoints &lt; 0 ("Reward points must be a non-negative integer") (AC4).
    /// </returns>
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

    /// <summary>
    /// API Ẩn (Soft delete) gói dịch vụ theo ID (US-08).
    /// Endpoint: PATCH /api/service-packages/{id}/deactivate
    /// - Người thực hiện: Được
    /// </summary>
    /// <param name="id">
    /// Int - NGUỒN: FE truyền qua Route Parameter (/api/service-packages/{id}/deactivate) - Mã ID gói dịch vụ cần ẩn.
    /// </param>
    /// <returns>
    /// ApiResponse&lt;string&gt;.
    /// - 200 OK: Ẩn gói thành công (AC1).
    /// - 404 Not Found: Nếu không tìm thấy package_id ("Package not found") (AC2).
    /// </returns>
    [HttpPatch("{id:int}/deactivate")]
    public async Task<IActionResult> DeactivatePackage(int id)
    {
        var success = await _servicePackageService.DeactivatePackageAsync(id);

        // AC2 — Gói không tồn tại
        if (!success)
        {
            return NotFound(ApiResponse<string>.Fail(404, "Package not found"));
        }

        // AC1 — Ẩn gói thành công
        return Ok(ApiResponse<string>.Ok("Tắt mở bán gói dịch vụ thành công.", "Ẩn gói thành công."));
    }

    /// <summary>
    /// API Mở lại (Kích hoạt lại) gói dịch vụ theo ID.
    /// Endpoint: PATCH /api/service-packages/{id}/activate
    /// - Người thực hiện: Được
    /// </summary>
    /// <param name="id">
    /// Int - NGUỒN: FE truyền qua Route Parameter (/api/service-packages/{id}/activate) - Mã ID gói dịch vụ cần mở lại.
    /// </param>
    /// <returns>
    /// ApiResponse&lt;string&gt;.
    /// - 200 OK: Mở lại gói thành công.
    /// - 404 Not Found: Nếu không tìm thấy package_id ("Package not found").
    /// </returns>
    [HttpPatch("{id:int}/activate")]
    public async Task<IActionResult> ActivatePackage(int id)
    {
        var success = await _servicePackageService.ActivatePackageAsync(id);

        if (!success)
        {
            return NotFound(ApiResponse<string>.Fail(404, "Package not found"));
        }

        return Ok(ApiResponse<string>.Ok("Mở bán lại gói dịch vụ thành công.", "Kích hoạt gói thành công."));
    }
}
