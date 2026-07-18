using CarWashBooking.Data;
using CarWashBooking.Models.DTOs;
using CarWashBooking.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarWashBooking.Services;

public class PaymentService : IPaymentService
{
    private readonly CarWashDbContext _dbContext;

    public PaymentService(CarWashDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /**
     * // comment by Tài: Lấy tổng điểm thưởng hiện tại của khách hàng.
     * @author Tài
     * @version 1.0
     */
    public async Task<RewardPointsResponse> GetRewardPointsAsync(int userId)
    {
        var user = await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new KeyNotFoundException("Khách hàng không tồn tại.");
        }

        return new RewardPointsResponse
        {
            TotalPoints = user.TotalPoints
        };
    }

    /**
     * // comment by Tài: Thực hiện thanh toán cho booking, hỗ trợ áp dụng điểm thưởng giảm giá.
     * @author Tài
     * @version 1.0
     */
    public async Task<PaymentResponse> ProcessPaymentAsync(int userId, PayRequest request)
    {
        // 1. Tìm booking kèm theo thông tin Package để tính toán
        var booking = await _dbContext.Bookings
            .Include(b => b.Package)
            .FirstOrDefaultAsync(b => b.Id == request.BookingId);

        if (booking == null)
        {
            throw new KeyNotFoundException("Không tìm thấy booking.");
        }

        // 2. Kiểm tra chủ sở hữu booking (AC2 US-14)
        if (booking.CustomerId != userId)
        {
            throw new UnauthorizedAccessException("You do not have permission to pay for this booking");
        }

        // 3. Kiểm tra trạng thái booking (AC3 US-14)
        if (booking.Status != BookingStatus.PendingPayment)
        {
            throw new InvalidOperationException("This booking is not pending payment");
        }

        // 4. Tìm package hiện tại để lấy RewardPoints (lấy giá trị reward_points hiện tại của gói tại thời điểm thanh toán)
        var package = await _dbContext.Packages.FindAsync(booking.PackageId);
        if (package == null)
        {
            throw new KeyNotFoundException("Không tìm thấy gói dịch vụ tương ứng.");
        }

        // Tìm thông tin khách hàng để cộng/trừ điểm
        var user = await _dbContext.Users.FindAsync(userId);
        if (user == null)
        {
            throw new KeyNotFoundException("Không tìm thấy thông tin khách hàng.");
        }

        // 5. Thực hiện lưu transaction cập nhật trạng thái và điểm thưởng
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            decimal amountPaid = package.Price;
            int pointsUsed = 0;

            // Cập nhật trạng thái booking
            booking.Status = BookingStatus.Paid;
            booking.PaidAt = DateTime.UtcNow;
            booking.AmountPaid = amountPaid;
            booking.PointsUsed = pointsUsed;
            booking.PointsEarned = package.RewardPoints;

            // Cộng điểm thưởng tích lũy của gói dịch vụ vào tài khoản user
            user.TotalPoints += package.RewardPoints;

            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return new PaymentResponse
            {
                BookingId = booking.Id,
                AmountPaid = amountPaid,
                PointsUsed = pointsUsed,
                PointsEarned = booking.PointsEarned,
                Status = booking.Status.ToString() // "Paid"
            };
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
