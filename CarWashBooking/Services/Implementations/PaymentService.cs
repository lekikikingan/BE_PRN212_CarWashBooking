using CarWashBooking.Data;
using CarWashBooking.Models.DTOs;
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
     * // comment by Tài: Thực hiện thanh toán cho booking, hỗ trợ áp dụng điểm thưởng giảm giá (sẽ implement ở các Milestone tiếp theo).
     * @author Tài
     * @version 1.0
     */
    public Task<PaymentResponse> ProcessPaymentAsync(int userId, PayRequest request)
    {
        throw new NotImplementedException();
    }
}
