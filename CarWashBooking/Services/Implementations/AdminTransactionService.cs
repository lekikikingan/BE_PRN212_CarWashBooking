using CarWashBooking.Data;
using CarWashBooking.Models.DTOs;
using CarWashBooking.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarWashBooking.Services;

/// <summary>
/// Lớp triển khai nghiệp vụ quản lý giao dịch cho Admin.
/// US-21 (Transaction List), US-22 (Filter Transaction by Status/Date), US-23 (Mark Booking Completed).
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

    /// <inheritdoc />
    public async Task<CompleteBookingResultDto?> CompleteBookingAsync(int bookingId)
    {
        var booking = await _dbContext.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);

        // Không tồn tại → Controller trả 404
        if (booking == null)
        {
            return null;
        }

        // BR-23 EXCEPTION 1 — Chỉ cho phép hoàn thành booking đang ở trạng thái PAID
        if (booking.Status != BookingStatus.Paid)
        {
            throw new InvalidOperationException("Chỉ có thể xác nhận hoàn thành cho booking đã thanh toán");
        }

        // BR-23 RULE 1 — Chuyển sang COMPLETED và ghi thời điểm hoàn thành
        booking.Status = BookingStatus.Completed;
        booking.CompletedAt = DateTime.UtcNow;
        await _dbContext.SaveChangesAsync();

        return new CompleteBookingResultDto
        {
            BookingId = booking.Id,
            Status = booking.Status.ToString().ToUpperInvariant(),
            CompletedAt = booking.CompletedAt
        };
    }
}
