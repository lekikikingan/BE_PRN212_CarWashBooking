namespace CarWashBooking.Models.DTOs;

/// <summary>
/// DTO chứa thông tin điểm thưởng của khách hàng.
/// </summary>
public class RewardPointsResponse
{
    public int TotalPoints { get; set; }
}

/// <summary>
/// DTO gửi yêu cầu thanh toán.
/// </summary>
public class PayRequest
{
    public int BookingId { get; set; }
    public int? PointsUsed { get; set; }
}

/// <summary>
/// DTO trả về kết quả thanh toán thành công.
/// </summary>
public class PaymentResponse
{
    public int BookingId { get; set; }
    public decimal AmountPaid { get; set; }
    public int PointsUsed { get; set; }
    public int PointsEarned { get; set; }
    public string Status { get; set; } = null!;
}
