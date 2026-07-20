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
    /// Lấy danh sách giao dịch (booking PAID/COMPLETED), sắp xếp theo PaidAt giảm dần (US-21 / BR-21).
    /// </summary>
    /// <returns>Danh sách TransactionItemDto. Trả về danh sách rỗng nếu không có giao dịch nào.</returns>
    Task<List<TransactionItemDto>> GetTransactionsAsync();
}
