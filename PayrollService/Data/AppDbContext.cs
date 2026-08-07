using Microsoft.EntityFrameworkCore;
using PayrollService.Models;

namespace PayrollService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Payslip> Payslips { get; set; }
    public DbSet<EmployeeLoan> EmployeeLoans { get; set; }
    public DbSet<TaxBracket> TaxBrackets { get; set; }
    public DbSet<CountryPolicy> CountryPolicies { get; set; }
    public DbSet<AgeBracket> AgeBrackets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ── Payslip decimal precision ─────────────────────
        modelBuilder.Entity<Payslip>(e =>
        {
            e.Property(p => p.BasicSalary).HasColumnType("decimal(18,2)");
            e.Property(p => p.Allowance).HasColumnType("decimal(18,2)");
            e.Property(p => p.OvertimePay).HasColumnType("decimal(18,2)");
            e.Property(p => p.YearEndBonus).HasColumnType("decimal(18,2)");
            e.Property(p => p.ThirteenthMonth).HasColumnType("decimal(18,2)");
            e.Property(p => p.GrossSalary).HasColumnType("decimal(18,2)");
            e.Property(p => p.TaxDeduction).HasColumnType("decimal(18,2)");
            e.Property(p => p.SocialSecurity).HasColumnType("decimal(18,2)");
            e.Property(p => p.CpfEmployee).HasColumnType("decimal(18,2)");
            e.Property(p => p.CpfEmployer).HasColumnType("decimal(18,2)");
            e.Property(p => p.LoanDeduction).HasColumnType("decimal(18,2)");
            e.Property(p => p.OtherDeduction).HasColumnType("decimal(18,2)");
            e.Property(p => p.TotalDeduction).HasColumnType("decimal(18,2)");
            e.Property(p => p.NetSalary).HasColumnType("decimal(18,2)");
        });

        // ── EmployeeLoan decimal precision ────────────────
        modelBuilder.Entity<EmployeeLoan>(e =>
        {
            e.Property(l => l.TotalLoanAmount).HasColumnType("decimal(18,2)");
            e.Property(l => l.MonthlyDeduction).HasColumnType("decimal(18,2)");
            e.Property(l => l.RemainingBalance).HasColumnType("decimal(18,2)");
        });

        // ── TaxBracket decimal precision ──────────────────
        modelBuilder.Entity<TaxBracket>(e =>
        {
            e.Property(t => t.MinIncome).HasColumnType("decimal(18,2)");
            e.Property(t => t.MaxIncome).HasColumnType("decimal(18,2)");
            e.Property(t => t.TaxRate).HasColumnType("decimal(5,2)");
        });

        // ── CountryPolicy decimal precision ───────────────
        modelBuilder.Entity<CountryPolicy>(e =>
        {
            e.Property(c => c.SocialContributionEmployeeRate)
                .HasColumnType("decimal(5,2)");
            e.Property(c => c.SocialContributionEmployerRate)
                .HasColumnType("decimal(5,2)");
        });

        // ── AgeBracket decimal precision ──────────────────
        modelBuilder.Entity<AgeBracket>(e =>
        {
            e.Property(a => a.EmployeeRate).HasColumnType("decimal(5,2)");
            e.Property(a => a.EmployerRate).HasColumnType("decimal(5,2)");
        });

        // ── Seed Country Policies ─────────────────────────
        modelBuilder.Entity<CountryPolicy>().HasData(
            new CountryPolicy
            {
                Id = 1,
                CountryCode = "MM",
                CountryName = "Myanmar",
                Currency = "MMK",
                FlagEmoji = "🇲🇲",
                SocialContributionLabel = "SSB",
                SocialContributionEmployeeRate = 2,
                SocialContributionEmployerRate = 3,
                HasProgressiveTax = true,
                HasAgeBased = false,
                IsActive = true
            },
            new CountryPolicy
            {
                Id = 2,
                CountryCode = "SG",
                CountryName = "Singapore",
                Currency = "SGD",
                FlagEmoji = "🇸🇬",
                SocialContributionLabel = "CPF",
                SocialContributionEmployeeRate = 0,
                SocialContributionEmployerRate = 0,
                HasProgressiveTax = false,
                HasAgeBased = true,
                IsActive = true
            },
            new CountryPolicy
            {
                Id = 3,
                CountryCode = "TH",
                CountryName = "Thailand",
                Currency = "THB",
                FlagEmoji = "🇹🇭",
                SocialContributionLabel = "SSF",
                SocialContributionEmployeeRate = 5,
                SocialContributionEmployerRate = 5,
                HasProgressiveTax = true,
                HasAgeBased = false,
                IsActive = true
            },
            new CountryPolicy
            {
                Id = 4,
                CountryCode = "MY",
                CountryName = "Malaysia",
                Currency = "MYR",
                FlagEmoji = "🇲🇾",
                SocialContributionLabel = "EPF",
                SocialContributionEmployeeRate = 11,
                SocialContributionEmployerRate = 13,
                HasProgressiveTax = true,
                HasAgeBased = false,
                IsActive = true
            },
            new CountryPolicy
            {
                Id = 5,
                CountryCode = "PH",
                CountryName = "Philippines",
                Currency = "PHP",
                FlagEmoji = "🇵🇭",
                SocialContributionLabel = "SSS",
                SocialContributionEmployeeRate = 4.5m,
                SocialContributionEmployerRate = 9.5m,
                HasProgressiveTax = true,
                HasAgeBased = false,
                IsActive = true
            }
        );

        // ── Seed Myanmar Tax Brackets ─────────────────────
        modelBuilder.Entity<TaxBracket>().HasData(
            new TaxBracket
            {
                Id = 1,
                Country = "MM",
                MinIncome = 0,
                MaxIncome = 4800000,
                TaxRate = 0,
                Description = "0% - Up to 4.8M MMK"
            },
            new TaxBracket
            {
                Id = 2,
                Country = "MM",
                MinIncome = 4800001,
                MaxIncome = 10000000,
                TaxRate = 5,
                Description = "5% - 4.8M to 10M MMK"
            },
            new TaxBracket
            {
                Id = 3,
                Country = "MM",
                MinIncome = 10000001,
                MaxIncome = 20000000,
                TaxRate = 10,
                Description = "10% - 10M to 20M MMK"
            },
            new TaxBracket
            {
                Id = 4,
                Country = "MM",
                MinIncome = 20000001,
                MaxIncome = 30000000,
                TaxRate = 15,
                Description = "15% - 20M to 30M MMK"
            },
            new TaxBracket
            {
                Id = 5,
                Country = "MM",
                MinIncome = 30000001,
                MaxIncome = 999999999,
                TaxRate = 20,
                Description = "20% - Above 30M MMK"
            },

            // ── Thailand Tax Brackets ─────────────────────
            new TaxBracket
            {
                Id = 6,
                Country = "TH",
                MinIncome = 0,
                MaxIncome = 150000,
                TaxRate = 0,
                Description = "0% - Up to 150,000 THB"
            },
            new TaxBracket
            {
                Id = 7,
                Country = "TH",
                MinIncome = 150001,
                MaxIncome = 300000,
                TaxRate = 5,
                Description = "5% - 150K to 300K THB"
            },
            new TaxBracket
            {
                Id = 8,
                Country = "TH",
                MinIncome = 300001,
                MaxIncome = 500000,
                TaxRate = 10,
                Description = "10% - 300K to 500K THB"
            },
            new TaxBracket
            {
                Id = 9,
                Country = "TH",
                MinIncome = 500001,
                MaxIncome = 750000,
                TaxRate = 15,
                Description = "15% - 500K to 750K THB"
            },
            new TaxBracket
            {
                Id = 10,
                Country = "TH",
                MinIncome = 750001,
                MaxIncome = 1000000,
                TaxRate = 20,
                Description = "20% - 750K to 1M THB"
            },
            new TaxBracket
            {
                Id = 11,
                Country = "TH",
                MinIncome = 1000001,
                MaxIncome = 2000000,
                TaxRate = 25,
                Description = "25% - 1M to 2M THB"
            },
            new TaxBracket
            {
                Id = 12,
                Country = "TH",
                MinIncome = 2000001,
                MaxIncome = 999999999,
                TaxRate = 35,
                Description = "35% - Above 2M THB"
            },

            // ── Malaysia Tax Brackets ─────────────────────
            new TaxBracket
            {
                Id = 13,
                Country = "MY",
                MinIncome = 0,
                MaxIncome = 5000,
                TaxRate = 0,
                Description = "0% - Up to 5,000 MYR"
            },
            new TaxBracket
            {
                Id = 14,
                Country = "MY",
                MinIncome = 5001,
                MaxIncome = 20000,
                TaxRate = 1,
                Description = "1% - 5K to 20K MYR"
            },
            new TaxBracket
            {
                Id = 15,
                Country = "MY",
                MinIncome = 20001,
                MaxIncome = 35000,
                TaxRate = 3,
                Description = "3% - 20K to 35K MYR"
            },
            new TaxBracket
            {
                Id = 16,
                Country = "MY",
                MinIncome = 35001,
                MaxIncome = 50000,
                TaxRate = 8,
                Description = "8% - 35K to 50K MYR"
            },
            new TaxBracket
            {
                Id = 17,
                Country = "MY",
                MinIncome = 50001,
                MaxIncome = 999999999,
                TaxRate = 13,
                Description = "13% - Above 50K MYR"
            },

            // ── Philippines Tax Brackets ──────────────────
            new TaxBracket
            {
                Id = 18,
                Country = "PH",
                MinIncome = 0,
                MaxIncome = 250000,
                TaxRate = 0,
                Description = "0% - Up to 250,000 PHP"
            },
            new TaxBracket
            {
                Id = 19,
                Country = "PH",
                MinIncome = 250001,
                MaxIncome = 400000,
                TaxRate = 20,
                Description = "20% - 250K to 400K PHP"
            },
            new TaxBracket
            {
                Id = 20,
                Country = "PH",
                MinIncome = 400001,
                MaxIncome = 800000,
                TaxRate = 25,
                Description = "25% - 400K to 800K PHP"
            },
            new TaxBracket
            {
                Id = 21,
                Country = "PH",
                MinIncome = 800001,
                MaxIncome = 2000000,
                TaxRate = 30,
                Description = "30% - 800K to 2M PHP"
            },
            new TaxBracket
            {
                Id = 22,
                Country = "PH",
                MinIncome = 2000001,
                MaxIncome = 8000000,
                TaxRate = 32,
                Description = "32% - 2M to 8M PHP"
            },
            new TaxBracket
            {
                Id = 23,
                Country = "PH",
                MinIncome = 8000001,
                MaxIncome = 999999999,
                TaxRate = 35,
                Description = "35% - Above 8M PHP"
            }
        );

        // ── Seed Singapore Age Brackets ───────────────────
        modelBuilder.Entity<AgeBracket>().HasData(
            new AgeBracket
            {
                Id = 1,
                CountryCode = "SG",
                MinAge = 0,
                MaxAge = 55,
                EmployeeRate = 20,
                EmployerRate = 17,
                Description = "Age 55 and below"
            },
            new AgeBracket
            {
                Id = 2,
                CountryCode = "SG",
                MinAge = 56,
                MaxAge = 60,
                EmployeeRate = 15,
                EmployerRate = 13,
                Description = "Age 56 to 60"
            },
            new AgeBracket
            {
                Id = 3,
                CountryCode = "SG",
                MinAge = 61,
                MaxAge = 65,
                EmployeeRate = 9.5m,
                EmployerRate = 9,
                Description = "Age 61 to 65"
            },
            new AgeBracket
            {
                Id = 4,
                CountryCode = "SG",
                MinAge = 66,
                MaxAge = 999,
                EmployeeRate = 7,
                EmployerRate = 7.5m,
                Description = "Age above 65"
            }
        );
    }
}