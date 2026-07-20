using CarWashBooking.Data;
using CarWashBooking.Models.DTOs;
using CarWashBooking.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarWashBooking.Services;

/// <summary>
/// Lớp triển khai nghiệp vụ quản lý giao dịch cho Admin.
/// US-23 (Mark Booking Completed).
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
