using CarWashBooking.Models.DTOs;

namespace CarWashBooking.Services;

/// <summary>
/// Quản lý giao dịch từ phía Admin: xem, lọc và xác nhận hoàn thành.
/// Chỉ hiển thị booking có status PAID hoặc COMPLETED.
/// US-21 (Transaction List), US-22 (Filter by Status/Date), US-23 (Mark Completed).
/// </summary>
public interface IAdminTransactionService
{
    /// <summary>
    /// Lấy danh sách giao dịch (booking PAID/COMPLETED) theo điều kiện lọc (US-22 / BR-22).
    /// Khi filter null hoặc mọi trường của filter null → trả về toàn bộ danh sách (US-21 / BR-21).
    /// </summary>
    /// <param name="filter">Điều kiện lọc theo trạng thái và/hoặc khoảng ngày. Có thể null.</param>
    /// <returns>Danh sách TransactionItemDto thỏa điều kiện, sắp xếp theo PaidAt giảm dần.</returns>
    Task<List<TransactionItemDto>> GetTransactionsAsync(TransactionFilterDto? filter = null);

    /// <summary>
    /// Xác nhận hoàn thành một booking đang ở trạng thái PAID (US-23 / BR-23).
    /// </summary>
    /// <param name="bookingId">Mã booking cần xác nhận hoàn thành.</param>
    /// <returns>
    /// CompleteBookingResultDto nếu cập nhật thành công (booking đang PAID → COMPLETED);
    /// null nếu booking không tồn tại (Controller trả 404);
    /// ném InvalidOperationException nếu booking không ở trạng thái PAID (Controller trả 409).
    /// </returns>
    Task<CompleteBookingResultDto?> CompleteBookingAsync(int bookingId);
}
