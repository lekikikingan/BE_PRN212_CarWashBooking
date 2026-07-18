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
    /// <param name="dbContext">CarWashDbContext - NGUỒN: DI Container - DbContext kết nối CSDL.</param>
    public ServicePackageService(CarWashDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <summary>
    /// Tạo gói dịch vụ mới trong CSDL với is_active = true (US-05).
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
    /// <returns>Danh sách PackageResponseDto.</returns>
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
}
