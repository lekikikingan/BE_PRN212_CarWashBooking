namespace CarWashBooking.Models.Entities;

/// <summary>
/// Trạng thái vòng đời của một booking.
///
/// Luồng trạng thái:
///   PENDING_PAYMENT → PAID → COMPLETED
///                  ↘         ↘
///               CANCELLED  CANCELLED
/// </summary>
public enum BookingStatus
{
    /// <summary>Vừa tạo booking, chờ khách thanh toán.</summary>
    PendingPayment = 0,

    /// <summary>Đã thanh toán, chờ ngày hẹn.</summary>
    Paid = 1,

    /// <summary>Dịch vụ đã hoàn thành. Admin cập nhật sau khi rửa xe xong.</summary>
    Completed = 2,

    /// <summary>Booking đã bị hủy. Slot giờ này được giải phóng cho người khác đặt.</summary>
    Cancelled = 3
}
