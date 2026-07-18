using CarWashBooking.Data;
using CarWashBooking.Models.DTOs;
using CarWashBooking.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarWashBooking.Services;

/// <summary>
/// Lớp triển khai các dịch vụ quản lý gói rửa xe cho Admin.
/// - Người thực hiện: Được
/// - Ngày tạo: 18/07/2026
/// </summary>
public class ServicePackageService : IServicePackageService
{
    private readonly CarWashDbContext _dbContext;

    /// <summary>
    /// Khởi tạo ServicePackageService với CarWashDbContext được Inject qua Constructor.
    /// - Người thực hiện: Được
    /// </summary>
    /// <param name="dbContext">CarWashDbContext - NGUỒN: DI Container - DbContext kết nối CSDL.</param>
    public ServicePackageService(CarWashDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Tạo gói dịch vụ mới trong CSDL với is_active = true (US-05 / BR-05).
    /// - Người thực hiện: Được
    /// </summary>
    /// <param name="requestDto">CreatePackageRequestDto - NGUỒN: Controller truyền vào - Thông tin gói cần tạo.</param>
    /// <returns>PackageResponseDto chứa thông tin gói vừa tạo.</returns>
    public async Task<PackageResponseDto> CreatePackageAsync(CreatePackageRequestDto requestDto)
    {
        var package = new ServicePackage
        {
            Name = requestDto.Name.Trim(),
            Description = requestDto.Description?.Trim(),
            Price = requestDto.Price,
            RewardPoints = requestDto.RewardPoints,
            IsActive = true // AC1 / BR-05: Gói mới tạo mặc định is_active = true
        };

        _dbContext.Packages.Add(package);
        await _dbContext.SaveChangesAsync();

        return new PackageResponseDto
        {
            Id = package.Id,
            Name = package.Name,
            Description = package.Description,
            Price = package.Price,
            RewardPoints = package.RewardPoints,
            IsActive = package.IsActive
        };
    }

    /// <summary>
    /// Lấy toàn bộ danh sách gói dịch vụ trong CSDL không lọc theo is_active, sắp xếp theo ID tăng dần (US-06 / BR-06).
    /// - Người thực hiện: Được
    /// </summary>
    /// <returns>Danh sách PackageResponseDto sắp xếp theo id ASC.</returns>
    public async Task<List<PackageResponseDto>> GetAllPackagesAsync()
    {
        return await _dbContext.Packages
            .AsNoTracking()
            .OrderBy(p => p.Id) // BR-06: Sắp xếp theo id tăng dần (ORDER BY id ASC)
            .Select(p => new PackageResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                RewardPoints = p.RewardPoints,
                IsActive = p.IsActive
            })
            .ToListAsync();
    }

    /// <summary>
    /// Lấy thông tin chi tiết gói dịch vụ theo ID. Trả về null nếu không tìm thấy (Hỗ trợ US-07 / BR-07).
    /// - Người thực hiện: Được
    /// </summary>
    /// <param name="id">Int - NGUỒN: Controller truyền vào - ID gói cần lấy.</param>
    /// <returns>PackageResponseDto nếu tìm thấy, null nếu không tìm thấy.</returns>
    public async Task<PackageResponseDto?> GetPackageByIdAsync(int id)
    {
        var p = await _dbContext.Packages.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (p == null) return null;

        return new PackageResponseDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            RewardPoints = p.RewardPoints,
            IsActive = p.IsActive
        };
    }

    /// <summary>
    /// Cập nhật thông tin gói dịch vụ. Giữ nguyên trường IsActive và các đơn hàng Booking đã tạo trước đó (US-07 / BR-07).
    /// - Người thực hiện: Được
    /// </summary>
    /// <param name="id">Int - NGUỒN: Controller truyền vào - ID gói cần sửa.</param>
    /// <param name="requestDto">UpdatePackageRequestDto - NGUỒN: Controller truyền vào - Thông tin gói sửa mới.</param>
    /// <returns>PackageResponseDto đã sửa thành công, null nếu không tìm thấy gói.</returns>
    public async Task<PackageResponseDto?> UpdatePackageAsync(int id, UpdatePackageRequestDto requestDto)
    {
        var package = await _dbContext.Packages.FirstOrDefaultAsync(p => p.Id == id);
        if (package == null)
        {
            return null; // AC2 / BR-07: Gói không tồn tại -> Trả về null
        }

        // Cập nhật thông tin bản ghi packages (trường IsActive giữ nguyên)
        package.Name = requestDto.Name.Trim();
        package.Description = requestDto.Description?.Trim();
        package.Price = requestDto.Price;
        package.RewardPoints = requestDto.RewardPoints;

        await _dbContext.SaveChangesAsync();

        return new PackageResponseDto
        {
            Id = package.Id,
            Name = package.Name,
            Description = package.Description,
            Price = package.Price,
            RewardPoints = package.RewardPoints,
            IsActive = package.IsActive
        };
    }

    /// <summary>
    /// Ẩn (soft delete) gói dịch vụ bằng cách cập nhật is_active = false (US-08 / BR-08).
    /// Các đơn booking hiện có tham chiếu đến package_id này giữ nguyên.
    /// - Người thực hiện: Được
    /// </summary>
    /// <param name="id">Int - NGUỒN: Controller truyền vào - ID gói dịch vụ cần ẩn.</param>
    /// <returns>True nếu ẩn thành công, False nếu không tìm thấy gói.</returns>
    public async Task<bool> DeactivatePackageAsync(int id)
    {
        var package = await _dbContext.Packages.FirstOrDefaultAsync(p => p.Id == id);
        if (package == null)
        {
            return false; // AC2 / BR-08: Gói không tồn tại -> Trả về false
        }

        package.IsActive = false; // AC1 / BR-08: Ẩn gói (soft delete)
        await _dbContext.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Mở lại gói dịch vụ bằng cách cập nhật is_active = true.
    /// - Người thực hiện: Được
    /// </summary>
    /// <param name="id">Int - NGUỒN: Controller truyền vào - ID gói cần mở lại.</param>
    /// <returns>True nếu mở lại thành công, False nếu không tìm thấy gói.</returns>
    public async Task<bool> ActivatePackageAsync(int id)
    {
        var package = await _dbContext.Packages.FirstOrDefaultAsync(p => p.Id == id);
        if (package == null)
        {
            return false; // Gói không tồn tại trong CSDL
        }

        package.IsActive = true; // Chuyển lại trạng thái mở bán (is_active = true)
        await _dbContext.SaveChangesAsync();
        return true;
    }
}
