using CarWashBooking.Models.DTOs;

namespace CarWashBooking.Services;

/// <summary>
/// Xử lý thanh toán giả lập và điểm thưởng.
/// Thanh toán là mock (không kết nối cổng thanh toán thật), thành công ngay lập tức.
/// US-14 (Pay), US-15 (Use Points for Discount), US-16 (Update Status), US-17 (Add Points).
/// Tỉ lệ quy đổi: 1 điểm = 1,000 VND.
/// </summary>
public interface IPaymentService
{
    /**
     * // comment by Tài: Lấy tổng điểm thưởng hiện tại của khách hàng.
     * @author Tài
     * @version 1.0
     */
    Task<RewardPointsResponse> GetRewardPointsAsync(int userId);

    /**
     * // comment by Tài: Thực hiện thanh toán cho booking, hỗ trợ áp dụng điểm thưởng giảm giá.
     * @author Tài
     * @version 1.0
     */
    Task<PaymentResponse> ProcessPaymentAsync(int userId, PayRequest request);
}
