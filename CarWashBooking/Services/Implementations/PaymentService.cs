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

        // 2. Kiểm tra chủ sở hữu booking 
        if (booking.CustomerId != userId)
        {
            throw new UnauthorizedAccessException("You do not have permission to pay for this booking");
        }

        // 3. Kiểm tra trạng thái booking 
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

        // 5. Kiểm tra và tính toán điểm sử dụng 
        int pointsUsed = request.PointsUsed ?? 0;

        // Số điểm nhập không hợp lệ (số âm)
        if (pointsUsed < 0)
        {
            throw new ArgumentException("Invalid number of points used");
        }

        // Số điểm nhập vượt quá điểm hiện có
        if (pointsUsed > user.TotalPoints)
        {
            throw new ArgumentException("The number of points used exceeds your available points");
        }

        // Số điểm quy đổi vượt quá giá gói (1 điểm = 1000đ)
        if (pointsUsed * 1000 > package.Price)
        {
            throw new ArgumentException("Points used cannot exceed the package price");
        }

        // Tính toán số tiền thực tế cần thanh toán sau khi giảm giá (1 điểm = 1000đ)
        decimal amountPaid = package.Price - (pointsUsed * 1000);

        // 6. Thực hiện lưu transaction cập nhật trạng thái và điểm thưởng
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        try
        {
            // Cập nhật trạng thái booking 
            booking.Status = BookingStatus.Paid;
            booking.PaidAt = DateTime.UtcNow;
            booking.AmountPaid = amountPaid;
            booking.PointsUsed = pointsUsed;
            booking.PointsEarned = package.RewardPoints;

            // Trừ điểm đã dùng và cộng điểm tích lũy của gói dịch vụ vào tài khoản user
            user.TotalPoints = user.TotalPoints - pointsUsed + package.RewardPoints;

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
