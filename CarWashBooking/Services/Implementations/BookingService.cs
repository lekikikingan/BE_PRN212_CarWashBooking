using CarWashBooking.Data;
using CarWashBooking.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarWashBooking.Services;

public class BookingService : IBookingService
{
    private readonly CarWashDbContext _dbContext;

    public BookingService(CarWashDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /**
     * // comment by Tài: Hủy lịch đặt xe.
     * @author Tài
     * @version 1.0
     */
    public async Task CancelBookingAsync(int userId, int bookingId)
    {
        var booking = await _dbContext.Bookings
            .FirstOrDefaultAsync(b => b.Id == bookingId);

        if (booking == null)
        {
            throw new KeyNotFoundException("Không tìm thấy booking.");
        }

        // Kiểm tra quyền sở hữu booking
        if (booking.CustomerId != userId)
        {
            throw new UnauthorizedAccessException("You do not have permission to cancel this booking");
        }

        // Kiểm tra trạng thái booking
        if (booking.Status != BookingStatus.PendingPayment)
        {
            throw new InvalidOperationException("Only bookings pending payment can be cancelled");
        }

        // Cập nhật trạng thái hủy
        booking.Status = BookingStatus.Cancelled;
        booking.CancelledAt = DateTime.UtcNow;

        await _dbContext.SaveChangesAsync();
    }
}
