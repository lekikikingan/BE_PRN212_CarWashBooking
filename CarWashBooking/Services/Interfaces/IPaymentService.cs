namespace CarWashBooking.Services;

/// <summary>
/// Xử lý thanh toán giả lập và điểm thưởng.
/// Thanh toán là mock (không kết nối cổng thanh toán thật), thành công ngay lập tức.
/// US-14 (Pay), US-15 (Use Points for Discount), US-16 (Update Status), US-17 (Add Points).
/// Tỉ lệ quy đổi: 1 điểm = 1,000 VND.
/// </summary>
public interface IPaymentService
{
}
