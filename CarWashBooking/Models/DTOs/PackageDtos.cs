namespace CarWashBooking.Models.DTOs;

/// <summary>
/// DTO nhận dữ liệu khi Admin tạo mới một gói dịch vụ (US-05).
/// </summary>
public class CreatePackageRequestDto
{
    /// <summary>Tên gói dịch vụ. Bắt buộc không được để trống (AC4).</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Mô tả chi tiết gói dịch vụ (Có thể để trống).</summary>
    public string? Description { get; set; }

    /// <summary>Giá gói dịch vụ. Bắt buộc lớn hơn 0 (AC2).</summary>
    public decimal Price { get; set; }

    /// <summary>Số điểm thưởng khi mua gói. Bắt buộc là số nguyên không âm (AC3).</summary>
    public int RewardPoints { get; set; }
}

/// <summary>
/// DTO nhận dữ liệu khi Admin chỉnh sửa gói dịch vụ (US-07).
/// </summary>
public class UpdatePackageRequestDto
{
    /// <summary>Tên gói dịch vụ mới. Bắt buộc không để trống.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Mô tả chi tiết mới của gói dịch vụ.</summary>
    public string? Description { get; set; }

    /// <summary>Giá mới của gói dịch vụ. Bắt buộc lớn hơn 0.</summary>
    public decimal Price { get; set; }

    /// <summary>Số điểm thưởng mới của gói dịch vụ. Bắt buộc là số nguyên không âm.</summary>
    public int RewardPoints { get; set; }
}

/// <summary>
/// DTO trả về thông tin chi tiết gói dịch vụ cho Client.
/// </summary>
public class PackageResponseDto
{
    /// <summary>Mã định danh duy nhất của gói dịch vụ.</summary>
    public int Id { get; set; }

    /// <summary>Tên gói dịch vụ.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Mô tả chi tiết gói dịch vụ.</summary>
    public string? Description { get; set; }

    /// <summary>Giá gói dịch vụ (VND).</summary>
    public decimal Price { get; set; }

    /// <summary>Số điểm thưởng tích lũy.</summary>
    public int RewardPoints { get; set; }

    /// <summary>Trạng thái hiển thị (true: Mở bán, false: Ẩn/Soft delete).</summary>
    public bool IsActive { get; set; }
}
