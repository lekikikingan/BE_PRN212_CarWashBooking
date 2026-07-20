using CarWashBooking.Data;
using CarWashBooking.Models.DTOs;
using Microsoft.EntityFrameworkCore;

namespace CarWashBooking.Services;

/// <summary>
/// Lớp triển khai nghiệp vụ quản lý khách hàng cho Admin.
/// US-19 (Customer List), US-20 (Customer Detail).
/// </summary>
public class AdminCustomerService : IAdminCustomerService
{
    private readonly CarWashDbContext _dbContext;

    /// <summary>
    /// Khởi tạo AdminCustomerService với CarWashDbContext được Inject qua Constructor.
    /// </summary>
    /// <param name="dbContext">CarWashDbContext - NGUỒN: DI Container - DbContext kết nối CSDL.</param>
    public AdminCustomerService(CarWashDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<List<CustomerListItemDto>> GetCustomersAsync()
    {
        // BR-19 — Chỉ lấy user có role = CUSTOMER, sắp xếp mới nhất trước
        return await _dbContext.Users
            .AsNoTracking()
            .Where(u => u.Role.Name == "CUSTOMER")
            .OrderByDescending(u => u.CreatedAt)
            .Select(u => new CustomerListItemDto
            {
                Id = u.Id,
                Email = u.Email,
                FullName = u.FullName,
                Phone = u.Phone,
                TotalPoints = u.TotalPoints
            })
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<CustomerDetailDto?> GetCustomerDetailAsync(int customerId)
    {
        // BR-20 — Chỉ trả về nếu tồn tại và đúng role CUSTOMER, ngược lại null (Controller → 404)
        return await _dbContext.Users
            .AsNoTracking()
            .Where(u => u.Id == customerId && u.Role.Name == "CUSTOMER")
            .Select(u => new CustomerDetailDto
            {
                FullName = u.FullName,
                Phone = u.Phone,
                Email = u.Email,
                LicensePlate = u.LicensePlate,
                TotalPoints = u.TotalPoints
            })
            .FirstOrDefaultAsync();
    }
}
