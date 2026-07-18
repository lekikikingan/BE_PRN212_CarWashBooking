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
    /// <param name="requestDto">CreatePackageRequestDto - DTO chứa thông tin tên, mô tả, giá và điểm thưởng.</param>
    /// <returns>PackageResponseDto chứa thông tin gói vừa tạo.</returns>
    Task<PackageResponseDto> CreatePackageAsync(CreatePackageRequestDto requestDto);
}
