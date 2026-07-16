using CarWashBooking.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarWashBooking.Data;

public class CarWashDbContext : DbContext
{
    public CarWashDbContext(DbContextOptions<CarWashDbContext> options) : base(options) { }

    public DbSet<Role> Roles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<ServicePackage> Packages { get; set; }
    public DbSet<TimeSlot> TimeSlots { get; set; }
    public DbSet<Booking> Bookings { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ----- Role -----
        modelBuilder.Entity<Role>(e =>
        {
            e.ToTable("Roles");
            e.Property(r => r.Name).HasMaxLength(20).IsRequired();
            e.HasIndex(r => r.Name).IsUnique().HasDatabaseName("UQ_Roles_Name");
        });

        // ----- User -----
        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("Users");
            e.Property(u => u.FullName).HasMaxLength(100).IsRequired();
            e.Property(u => u.Email).HasMaxLength(255).IsRequired();
            e.Property(u => u.PasswordHash).HasMaxLength(255).IsRequired();
            e.Property(u => u.Phone).HasMaxLength(10);
            e.Property(u => u.LicensePlate).HasMaxLength(20);
            e.Property(u => u.TotalPoints).HasDefaultValue(0);
            e.Property(u => u.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

            e.HasIndex(u => u.Email).IsUnique().HasDatabaseName("UQ_Users_Email");
            e.HasIndex(u => u.RoleId).HasDatabaseName("IX_Users_RoleId");

            e.HasOne(u => u.Role)
             .WithMany(r => r.Users)
             .HasForeignKey(u => u.RoleId)
             .OnDelete(DeleteBehavior.Restrict)
             .HasConstraintName("FK_Users_Roles");
        });

        // ----- Session -----
        modelBuilder.Entity<Session>(e =>
        {
            e.ToTable("Sessions");
            e.HasKey(s => s.SessionId);
            e.Property(s => s.SessionId).HasMaxLength(64);
            e.Property(s => s.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

            e.HasIndex(s => s.UserId).HasDatabaseName("IX_Sessions_UserId");

            e.HasOne(s => s.User)
             .WithMany(u => u.Sessions)
             .HasForeignKey(s => s.UserId)
             .OnDelete(DeleteBehavior.Cascade)
             .HasConstraintName("FK_Sessions_Users");
        });

        // ----- ServicePackage → dbo.Packages -----
        modelBuilder.Entity<ServicePackage>(e =>
        {
            e.ToTable("Packages");
            e.Property(p => p.Name).HasMaxLength(100).IsRequired();
            e.Property(p => p.Price).HasPrecision(12, 2);
            e.Property(p => p.IsActive).HasDefaultValue(true);

            e.HasIndex(p => p.IsActive).HasDatabaseName("IX_Packages_IsActive");
        });

        // ----- TimeSlot -----
        modelBuilder.Entity<TimeSlot>(e =>
        {
            e.ToTable("TimeSlots");
            e.HasIndex(t => t.SlotTime).IsUnique().HasDatabaseName("UQ_TimeSlots_SlotTime");
        });

        // ----- Booking -----
        modelBuilder.Entity<Booking>(e =>
        {
            e.ToTable("Bookings");
            e.Property(b => b.Status)
             .HasConversion<string>()
             .HasMaxLength(20)
             .HasDefaultValue(BookingStatus.PendingPayment);
            e.Property(b => b.AmountPaid).HasPrecision(12, 2);
            e.Property(b => b.PointsUsed).HasDefaultValue(0);
            e.Property(b => b.PointsEarned).HasDefaultValue(0);
            e.Property(b => b.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

            // Filtered unique index: one active booking per date/slot
            e.HasIndex(b => new { b.AppointmentDate, b.TimeSlotId })
             .IsUnique()
             .HasFilter("[Status] IN ('PendingPayment', 'Paid', 'Completed')")
             .HasDatabaseName("UX_Bookings_ActiveSlot");

            e.HasIndex(b => b.CustomerId).HasDatabaseName("IX_Bookings_CustomerId");
            e.HasIndex(b => b.PackageId).HasDatabaseName("IX_Bookings_PackageId");
            e.HasIndex(b => b.Status).HasDatabaseName("IX_Bookings_Status");

            e.HasOne(b => b.Customer)
             .WithMany(u => u.Bookings)
             .HasForeignKey(b => b.CustomerId)
             .OnDelete(DeleteBehavior.Restrict)
             .HasConstraintName("FK_Bookings_Customer");

            e.HasOne(b => b.Package)
             .WithMany(p => p.Bookings)
             .HasForeignKey(b => b.PackageId)
             .OnDelete(DeleteBehavior.Restrict)
             .HasConstraintName("FK_Bookings_Package");

            e.HasOne(b => b.TimeSlot)
             .WithMany(t => t.Bookings)
             .HasForeignKey(b => b.TimeSlotId)
             .OnDelete(DeleteBehavior.Restrict)
             .HasConstraintName("FK_Bookings_TimeSlots");
        });
    }
}
