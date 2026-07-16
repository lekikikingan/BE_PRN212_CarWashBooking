using CarWashBooking.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace CarWashBooking.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<CarWashDbContext>();

        await SeedRolesAsync(db);
        await SeedTimeSlots(db);
        await SeedUsersAsync(db);
        await SeedPackagesAsync(db);
    }

    private static async Task SeedRolesAsync(CarWashDbContext db)
    {
        if (await db.Roles.AnyAsync()) return;

        db.Roles.AddRange(
            new Role { Name = "CUSTOMER" },
            new Role { Name = "ADMIN" }
        );
        await db.SaveChangesAsync();
    }

    private static async Task SeedTimeSlots(CarWashDbContext db)
    {
        if (await db.TimeSlots.AnyAsync()) return;

        var slots = Enumerable.Range(0, 24)
            .Select(i => new TimeSlot { SlotTime = new TimeOnly(8, 0).AddMinutes(i * 30) });

        db.TimeSlots.AddRange(slots);
        await db.SaveChangesAsync();
    }

    private static async Task SeedUsersAsync(CarWashDbContext db)
    {
        if (await db.Users.AnyAsync()) return;

        var adminRole = await db.Roles.FirstAsync(r => r.Name == "ADMIN");
        var customerRole = await db.Roles.FirstAsync(r => r.Name == "CUSTOMER");

        db.Users.AddRange(
            new User
            {
                FullName = "Administrator",
                Email = "admin@autowash.com",
                PasswordHash = HashPassword("Admin@123"),
                RoleId = adminRole.Id,
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                FullName = "Nguyễn Văn A",
                Email = "nguyen.van.a@gmail.com",
                PasswordHash = HashPassword("Customer@123"),
                Phone = "0901234567",
                LicensePlate = "51A-12345",
                RoleId = customerRole.Id,
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                FullName = "Trần Thị B",
                Email = "tran.thi.b@gmail.com",
                PasswordHash = HashPassword("Customer@123"),
                Phone = "0912345678",
                LicensePlate = "59B-67890",
                RoleId = customerRole.Id,
                CreatedAt = DateTime.UtcNow
            }
        );
        await db.SaveChangesAsync();
    }

    private static async Task SeedPackagesAsync(CarWashDbContext db)
    {
        if (await db.Packages.AnyAsync()) return;

        db.Packages.AddRange(
            new ServicePackage
            {
                Name = "Basic",
                Description = "Rửa xe cơ bản bên ngoài, sạch bụi bẩn thông thường.",
                Price = 50000,
                RewardPoints = 5,
                IsActive = true
            },
            new ServicePackage
            {
                Name = "Medium",
                Description = "Rửa xe toàn diện, bao gồm lau kính, vành bánh xe và hút bụi nội thất.",
                Price = 100000,
                RewardPoints = 12,
                IsActive = true
            },
            new ServicePackage
            {
                Name = "Premium",
                Description = "Rửa xe cao cấp, đánh bóng sơn, vệ sinh nội thất toàn bộ.",
                Price = 200000,
                RewardPoints = 25,
                IsActive = true
            }
        );
        await db.SaveChangesAsync();
    }

    private static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes).ToLower();
    }
}
