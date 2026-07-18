using CarWashBooking.Data;
using CarWashBooking.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Cấu hình Swagger API Explorer
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<CarWashDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Cookie-based session authentication (không dùng JWT)
// SessionId được lưu trong cookie, validate qua bảng Sessions trong DB
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/api/auth/login";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });

builder.Services.AddAuthorization();

// Đăng ký service layer — team implement logic vào các class tương ứng trong Services/
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IServicePackageService, ServicePackageService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IAdminCustomerService, AdminCustomerService>();
builder.Services.AddScoped<IAdminTransactionService, AdminTransactionService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    // Bật Swagger & Swagger UI ở môi trường Development
    app.UseSwagger();
    app.UseSwaggerUI();

    await DbInitializer.SeedAsync(app.Services);
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
