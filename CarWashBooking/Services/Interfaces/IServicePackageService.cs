using CarWashBooking.Models.DTOs;

namespace CarWashBooking.Services;

/// <summary>
/// Interface định nghĩa các dịch vụ quản lý gói dịch vụ rửa xe cho Admin.
/// US-05 (Create), US-06 (List all - Admin), US-07 (Edit), US-08 (Deactivate).
/// - Người thực hiện: Được
/// - Ngày tạo: 18/07/2026
/// </summary>
public interface IServicePackageService
{
    /// <summary>
    /// Tạo một gói dịch vụ mới vào CSDL với trạng thái is_active = true (US-05).
    /// - Người thực hiện: Được
    /// </summary>
    /// <param name="requestDto">CreatePackageRequestDto - NGUỒN: Controller truyền vào - Thông tin gói cần tạo.</param>
    /// <returns>PackageResponseDto chứa thông tin gói vừa tạo.</returns>
    Task<PackageResponseDto> CreatePackageAsync(CreatePackageRequestDto requestDto);

    /// <summary>
    /// Lấy toàn bộ danh sách gói dịch vụ trong CSDL kể cả gói đang ẩn dành cho Admin (US-06).
    /// - Người thực hiện: Được
    /// </summary>
    /// <returns>Danh sách các PackageResponseDto.</returns>
    Task<List<PackageResponseDto>> GetAllPackagesAsync();

    /// <summary>
    /// Lấy thông tin chi tiết của 1 gói dịch vụ theo ID (Hỗ trợ US-07).
    /// - Người thực hiện: Được
    /// </summary>
    /// <param name="id">Int - NGUỒN: Controller truyền vào - ID gói dịch vụ cần tìm.</param>
    /// <returns>PackageResponseDto nếu thấy, null nếu không thấy.</returns>
    Task<PackageResponseDto?> GetPackageByIdAsync(int id);

    /// <summary>
    /// Chỉnh sửa tên, mô tả, giá hoặc điểm thưởng của gói đang tồn tại theo ID (US-07).
    /// - Người thực hiện: Được
    /// </summary>
    /// <param name="id">Int - NGUỒN: Controller truyền vào - ID gói dịch vụ cần sửa.</param>
    /// <param name="requestDto">UpdatePackageRequestDto - NGUỒN: Controller truyền vào - Thông tin sửa mới.</param>
    /// <returns>PackageResponseDto đã sửa thành công, null nếu không thấy ID.</returns>
    Task<PackageResponseDto?> UpdatePackageAsync(int id, UpdatePackageRequestDto requestDto);

    /// <summary>
    /// Ẩn (Soft delete) gói dịch vụ bằng cách cập nhật is_active = false (US-08).
    /// - Người thực hiện: Được
    /// </summary>
    /// <param name="id">Int - NGUỒN: Controller truyền vào - ID gói dịch vụ cần ẩn.</param>
    /// <returns>True nếu ẩn thành công, False nếu không tìm thấy gói.</returns>
    Task<bool> DeactivatePackageAsync(int id);

    /// <summary>
    /// Mở lại (Kích hoạt lại) gói dịch vụ bằng cách cập nhật is_active = true.
    /// - Người thực hiện: Được
    /// </summary>
    /// <param name="id">Int - NGUỒN: Controller truyền vào - ID gói dịch vụ cần mở lại.</param>
    /// <returns>True nếu mở lại thành công, False nếu không tìm thấy gói.</returns>
    Task<bool> ActivatePackageAsync(int id);
}
