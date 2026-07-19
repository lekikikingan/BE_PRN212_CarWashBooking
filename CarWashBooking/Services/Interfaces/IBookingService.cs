namespace CarWashBooking.Services;

/// <summary>
/// Xử lý đặt lịch rửa xe: xem slot khả dụng, tạo booking, xem lịch sử, hủy.
/// US-10 (Select Slot), US-11 (Confirm Booking), US-12 (History), US-13 (Cancel).
/// </summary>
public interface IBookingService
{
    /**
     * // comment by Tài: Hủy lịch đặt xe.
     * @author Tài
     * @version 1.0
     */
    Task CancelBookingAsync(int userId, int bookingId);
}
