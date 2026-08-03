using Microsoft.EntityFrameworkCore;
using IdentityService.Models;

namespace IdentityService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>().HasData(new User
        {
            Id = 1,
            FullName = "Super Admin",
            Email = "admin@hrms.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Role = "Admin",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });
    }
}