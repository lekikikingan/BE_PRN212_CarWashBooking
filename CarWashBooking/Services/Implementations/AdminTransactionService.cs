using CarWashBooking.Data;
using CarWashBooking.Models.DTOs;
using CarWashBooking.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarWashBooking.Services;

/// <summary>
/// Lớp triển khai nghiệp vụ quản lý giao dịch cho Admin.
/// US-22 (Filter Transaction by Status/Date).
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
    public async Task<List<TransactionItemDto>> GetTransactionsAsync(TransactionFilterDto? filter = null)
    {
        // BR-21 — Chỉ lấy booking đã thanh toán trở lên (PAID, COMPLETED)
        var query = _dbContext.Bookings
            .AsNoTracking()
            .Where(b => b.Status == BookingStatus.Paid || b.Status == BookingStatus.Completed);

        // BR-22 — Áp dụng bộ lọc (AND) nếu có
        if (filter != null)
        {
            // RULE 1 — Lọc theo trạng thái
            if (filter.Status.HasValue)
            {
                query = query.Where(b => b.Status == filter.Status.Value);
            }

            // RULE 2 — Lọc theo khoảng ngày hẹn
            if (filter.FromDate.HasValue)
            {
                query = query.Where(b => b.AppointmentDate >= filter.FromDate.Value);
            }

            if (filter.ToDate.HasValue)
            {
                query = query.Where(b => b.AppointmentDate <= filter.ToDate.Value);
            }
        }

        return await query
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
