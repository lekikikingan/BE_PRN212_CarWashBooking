namespace CarWashBooking.Models.Common;

/// <summary>
/// Chuẩn hóa cấu trúc phản hồi API (API Response Envelope) cho toàn bộ hệ thống.
/// </summary>
/// <typeparam name="T">Kiểu dữ liệu của Payload (Data).</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// Cho biết yêu cầu xử lý thành công (true) hay thất bại (false).
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Mã trạng thái HTTP (200, 400, 404, 500,...).
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Thông báo chi tiết dành cho Frontend hiển thị.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Dữ liệu phản hồi thực sự.
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Danh sách chi tiết các lỗi (nếu có).
    /// </summary>
    public List<string>? Errors { get; set; }

    /// <summary>
    /// Tạo phản hồi thành công.
    /// </summary>
    public static ApiResponse<T> Ok(T data, string message = "Thành công")
    {
        return new ApiResponse<T>
        {
            Success = true,
            StatusCode = 200,
            Message = message,
            Data = data,
            Errors = null
        };
    }

    /// <summary>
    /// Tạo phản hồi thất bại.
    /// </summary>
    public static ApiResponse<T> Fail(int statusCode, string message, List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            StatusCode = statusCode,
            Message = message,
            Data = default,
            Errors = errors
        };
    }
}
