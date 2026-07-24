using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using BCrypt.Net;
using CarWashBooking.Models;
using CarWashBooking.DTOs;
using Org.BouncyCastle.Crypto.Generators;

namespace CarWashBooking.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    // Giả lập database bằng List tĩnh trong bộ nhớ
    private static readonly List<User> Users = new List<User>();
    private static readonly List<Session> Sessions = new List<Session>();

    // ==========================================
    // MD-01 / BR-01 — ĐĂNG KÝ TÀI KHOẢN
    // ==========================================
    [HttpPost("register")]
    public IActionResult Register([FromBody] UserDto dto)
    {
        // RULE 1 — Kiểm tra định dạng email
        var emailRegex = @"^[^\s@]+@[^\s@]+\.[^\s@]+$";
        if (!Regex.IsMatch(dto.Email, emailRegex))
        {
            return BadRequest(new { message = "Email không đúng định dạng" });
        }

        // RULE 2 — Kiểm tra email duy nhất
        if (Users.Any(u => u.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase)))
        {
            return Conflict(new { message = "Email đã được sử dụng" });
        }

        // RULE 3 — Kiểm tra độ dài mật khẩu
        if (string.IsNullOrEmpty(dto.Password) || dto.Password.Length < 8)
        {
            return BadRequest(new { message = "Mật khẩu phải có tối thiểu 8 ký tự" });
        }

        // RULE 4 — Tạo tài khoản (Mã hóa mật khẩu bằng BCrypt)
        string salt = BCrypt.Net.BCrypt.GenerateSalt();
        string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password, salt);

        var newUser = new User
        {
            Id = Users.Count + 1,
            Email = dto.Email,
            PasswordHash = passwordHash,
            FullName = dto.FullName,
            Phone = dto.Phone
        };
        Users.Add(newUser);

        return StatusCode(201, new { message = "Đăng ký thành công!" });
    }

    // ==========================================
    // MD-01 / BR-02 — ĐĂNG NHẬP (TẠO COOKIE)
    // ==========================================
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginDto dto)
    {
        // RULE 1 & 2 — Xác thực email & mật khẩu
        var user = Users.FirstOrDefault(u => u.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase));
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        {
            return Unauthorized(new { message = "Email hoặc mật khẩu không đúng" });
        }

        // RULE 3 — Tạo phiên đăng nhập (Token ngẫu nhiên)
        string token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32)).ToLower();
        var expiresAt = DateTime.Now.AddDays(1);

        Sessions.Add(new Session
        {
            Id = token,
            UserId = user.Id,
            CreatedAt = DateTime.Now,
            ExpiresAt = expiresAt
        });

        // Gửi session_id về dưới dạng Cookie HttpOnly
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = expiresAt,
            Secure = false // false để chạy localhost
        };
        Response.Cookies.Append("session_id", token, cookieOptions);

        return Ok(new { message = "Đăng nhập thành công" });
    }

    // ==========================================
    // MD-01 / BR-03 — ĐĂNG XUẤT (HỦY COOKIE)
    // ==========================================
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        if (!Request.Cookies.TryGetValue("session_id", out string? token))
        {
            return Unauthorized(new { message = "Phiên đăng nhập không hợp lệ hoặc đã hết hạn" });
        }

        var session = Sessions.FirstOrDefault(s => s.Id == token && s.ExpiresAt > DateTime.Now);
        if (session == null)
        {
            return Unauthorized(new { message = "Phiên đăng nhập không hợp lệ hoặc đã hết hạn" });
        }

        // RULE 1 — Hủy phiên phía server
        Sessions.Remove(session);

        // RULE 2 — Xóa cookie phía client
        Response.Cookies.Delete("session_id");

        return Ok(new { message = "Đăng xuất thành công" });
    }

    // ==========================================
    // MD-02 / BR-04 — CẬP NHẬT THÔNG TIN CÁ NHÂN
    // ==========================================
    [HttpPut("profile")]
    public IActionResult UpdateProfile([FromBody] UserDto dto)
    {
        if (!Request.Cookies.TryGetValue("session_id", out string? token))
        {
            return Unauthorized(new { message = "Phiên đăng nhập không hợp lệ hoặc đã hết hạn" });
        }

        var session = Sessions.FirstOrDefault(s => s.Id == token && s.ExpiresAt > DateTime.Now);
        if (session == null)
        {
            return Unauthorized(new { message = "Phiên đăng nhập không hợp lệ hoặc đã hết hạn" });
        }

        var currentUser = Users.FirstOrDefault(u => u.Id == session.UserId);
        if (currentUser == null)
        {
            return NotFound(new { message = "Không tìm thấy người dùng" });
        }

        // RULE 1 — Kiểm tra định dạng số điện thoại
        var phoneRegex = @"^0\d{9}$";
        if (!string.IsNullOrEmpty(dto.Phone) && !Regex.IsMatch(dto.Phone, phoneRegex))
        {
            return BadRequest(new { message = "Số điện thoại không hợp lệ" });
        }

        // RULE 2 — Kiểm tra email mới không trùng người khác
        if (!dto.Email.Equals(currentUser.Email, StringComparison.OrdinalIgnoreCase))
        {
            bool emailExists = Users.Any(u => u.Id != currentUser.Id && u.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase));
            if (emailExists)
            {
                return Conflict(new { message = "Email đã được sử dụng" });
            }
        }

        // RULE 3 — Cập nhật thông tin vào hệ thống
        currentUser.FullName = dto.FullName;
        currentUser.Email = dto.Email;
        currentUser.Phone = dto.Phone;

        return Ok(new { message = "Cập nhật thông tin thành công!", user = new { currentUser.Email, currentUser.FullName, currentUser.Phone } });
    }
}