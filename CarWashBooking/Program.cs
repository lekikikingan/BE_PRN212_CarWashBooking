using CarWashBooking.Data;
using CarWashBooking.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<CarWashDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

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
    await DbInitializer.SeedAsync(app.Services);
}

app.MapControllers();

app.Run();
