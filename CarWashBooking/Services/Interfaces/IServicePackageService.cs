using CarWashBooking.Models.DTOs;

namespace CarWashBooking.Services;

/// <summary>
/// Quản lý gói dịch vụ: tạo, xem, sửa, ẩn gói.
/// US-05 (Create), US-06 (List all - Admin), US-07 (Edit), US-08 (Deactivate).
/// </summary>
public interface IServicePackageService
{
    /// <summary>
    /// Tạo một gói dịch vụ mới vào CSDL với trạng thái is_active = true (US-05).
    /// </summary>
    Task<PackageResponseDto> CreatePackageAsync(CreatePackageRequestDto requestDto);

    /// <summary>
    /// Lấy toàn bộ danh sách gói dịch vụ trong CSDL kể cả gói đang ẩn dành cho Admin (US-06).
    /// </summary>
    Task<List<PackageResponseDto>> GetAllPackagesAsync();

    /// <summary>
    /// Lấy thông tin chi tiết của 1 gói dịch vụ theo ID (Hỗ trợ US-07).
    /// </summary>
    Task<PackageResponseDto?> GetPackageByIdAsync(int id);

    /// <summary>
    /// Chỉnh sửa tên, mô tả, giá hoặc điểm thưởng của gói đang tồn tại theo ID (US-07).
    /// </summary>
    Task<PackageResponseDto?> UpdatePackageAsync(int id, UpdatePackageRequestDto requestDto);
}
