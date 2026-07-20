using CarWashBooking.Data;
using CarWashBooking.Models.DTOs;
using CarWashBooking.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarWashBooking.Services;

/// <summary>
/// Lớp triển khai nghiệp vụ quản lý giao dịch cho Admin.
/// US-21 (Transaction List).
/// </summary>
public class AdminTransactionService : IAdminTransactionService
{
    private readonly CarWashDbContext _dbContext;

    /// <summary>
    /// Khởi tạo AdminTransactionService với CarWashDbContext được Inject qua Constructor.
    /// </summary>
    /// <param name="dbContext">CarWashDbContext - NGUỒN: DI Container - DbContext kết nối CSDL.</param>
    public AdminTransactionService(CarWashDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<List<TransactionItemDto>> GetTransactionsAsync()
    {
        // BR-21 — Chỉ lấy booking đã thanh toán trở lên (PAID, COMPLETED), sắp xếp PaidAt giảm dần
        return await _dbContext.Bookings
            .AsNoTracking()
            .Where(b => b.Status == BookingStatus.Paid || b.Status == BookingStatus.Completed)
            .OrderByDescending(b => b.PaidAt)
            .Select(b => new TransactionItemDto
            {
                BookingId = b.Id,
                CustomerName = b.Customer.FullName,
                PackageName = b.Package.Name,
                AmountPaid = b.AmountPaid,
                PointsUsed = b.PointsUsed,
                PointsEarned = b.PointsEarned,
                Status = b.Status.ToString().ToUpperInvariant(),
                PaidAt = b.PaidAt
            })
            .ToListAsync();
    }
}
