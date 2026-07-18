using CarWashBooking.Data;
using CarWashBooking.Models.DTOs;
using CarWashBooking.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarWashBooking.Services;

/// <summary>
/// Lớp triển khai các dịch vụ quản lý gói rửa xe cho Admin.
/// </summary>
public class ServicePackageService : IServicePackageService
{
    private readonly CarWashDbContext _dbContext;

    /// <summary>
    /// Khởi tạo ServicePackageService với CarWashDbContext được Inject qua Constructor.
    /// </summary>
    public ServicePackageService(CarWashDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Tạo gói dịch vụ mới trong CSDL với is_active = true (US-05).
    /// </summary>
    public async Task<PackageResponseDto> CreatePackageAsync(CreatePackageRequestDto requestDto)
    {
        var package = new ServicePackage
        {
            Name = requestDto.Name.Trim(),
            Description = requestDto.Description?.Trim(),
            Price = requestDto.Price,
            RewardPoints = requestDto.RewardPoints,
            IsActive = true // AC1: Gói mới tạo mặc định is_active = true
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
    /// Lấy toàn bộ danh sách gói dịch vụ trong CSDL không lọc theo is_active (US-06).
    /// </summary>
    public async Task<List<PackageResponseDto>> GetAllPackagesAsync()
    {
        return await _dbContext.Packages
            .AsNoTracking()
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
    /// Lấy thông tin chi tiết gói dịch vụ theo ID. Trả về null nếu không tìm thấy.
    /// </summary>
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
    /// Cập nhật thông tin gói dịch vụ. Giữ nguyên trường IsActive và các đơn hàng Booking đã tạo trước đó (US-07).
    /// </summary>
    public async Task<PackageResponseDto?> UpdatePackageAsync(int id, UpdatePackageRequestDto requestDto)
    {
        var package = await _dbContext.Packages.FirstOrDefaultAsync(p => p.Id == id);
        if (package == null)
        {
            return null; // AC2 — Gói không tồn tại -> Trả về null
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
}
