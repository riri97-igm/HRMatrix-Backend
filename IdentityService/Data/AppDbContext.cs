using IdentityService.Models;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.Data
{
    public class AppDbContext : DbContext
    {
        // EF create "Users" table in the database
        public DbSet<User> Users { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Email must be unique - no duplicate accounts
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            //Seed a default admin user so can login right away 
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                FullName = "Super Admin",
                Email = "admin@hrms.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = "Addmin",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });
        }
        
    }
}
