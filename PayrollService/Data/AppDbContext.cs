using Microsoft.EntityFrameworkCore;
using PayrollService.Models;

namespace PayrollService.Properties.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) :base(options){}

    public DbSet<Payslip> Payslips { get; set; }
    public DbSet<EmployeeLoan> EmployeeLoans { get; set; }
    public DbSet<TaxBracket> TaxBrackets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Payslip decimal precision 
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
            e.Property(p => p.Cpfemployee).HasColumnType("decimal(18,2)");
            e.Property(p => p.Cpfemployer).HasColumnType("decimal(18,2)");
            e.Property(p => p.LoanDeduction).HasColumnType("decimal(18,2)");
            e.Property(p => p.OtherDeduction).HasColumnType("decimal(18,2)");
            e.Property(p => p.TotalDeduction).HasColumnType("decimal(18,2)");
            e.Property(p => p.NetSalary).HasColumnType("decimal(18,2)");
        });

        // EmployeeLoan decimal precision
        modelBuilder.Entity<EmployeeLoan>(e =>
        {
            e.Property(l => l.TotalLoanAmount).HasColumnType("decimal(18,2)");
            e.Property(l => l.MonthlyDeduction).HasColumnType("decimal(18,2)");
            e.Property(l => l.RemainingBalance).HasColumnType("decimal(18,2)");
        });

        // TaxBracket decimal precision 
        modelBuilder.Entity<TaxBracket>(e =>
        {
            e.Property(t => t.MinIncome).HasColumnType("decimal(18,2)");
            e.Property(t => t.MaxIncome).HasColumnType("decimal(18,2)");
            e.Property(t => t.TaxRate).HasColumnType("decimal(18,2)");
        });

        // Seed Myanmar Tax Brackets 
        modelBuilder.Entity<TaxBracket>().HasData(
            new TaxBracket
            {
                Id = 1,
                Country = "Myanmar",
                MinIncome = 0,
                MaxIncome = 4800000,
                TaxRate = 0,
                Description = "0% - Up to 4.8M MMK"
            },
            new TaxBracket
            {
                Id = 2,
                Country = "Myanmar",
                MinIncome = 4800001,
                MaxIncome = 10000000,
                TaxRate = 5,
                Description = "5% - 4.8M to 10M MMK"
            },
            new TaxBracket
            {
                Id = 3,
                Country = "Myanmar",
                MinIncome = 10000001,
                MaxIncome = 20000000,
                TaxRate = 10,
                Description = "10% - 10M to 20M MMK"
            },
            new TaxBracket
            {
                Id = 4,
                Country = "Myanmar",
                MinIncome = 20000001,
                MaxIncome = 30000000,
                TaxRate = 15,
                Description = "15% - 20M to 30M MMK"
            },
            new TaxBracket
            {
                Id = 5,
                Country = "Myanmar",
                MinIncome = 30000001,
                MaxIncome = 999999999,
                TaxRate = 20,
                Description = "20% - Above 30M MMK"
            },

            // Seed Singapore CPF Rates
            new TaxBracket
            {
                Id = 6,
                Country = "Singapore_Employee_55below",
                MinIncome = 0,
                MaxIncome = 999999999,
                TaxRate = 20,
                Description = "CPF Employee 20% (age 55 and below)"
            },
            new TaxBracket
            {
                Id = 7,
                Country = "Singapore_Employer_55below",
                MinIncome = 0,
                MaxIncome = 999999999,
                TaxRate = 17,
                Description = "CPF Employer 17% (age 55 and below)"
            },
            new TaxBracket
            {
                Id = 8,
                Country = "Singapore_Employee_55to60",
                MinIncome = 0,
                MaxIncome = 999999999,
                TaxRate = 15,
                Description = "CPF Employee 15% (age 55-60)"
            },
            new TaxBracket
            {
                Id = 9,
                Country = "Singapore_Employer_55to60",
                MinIncome = 0,
                MaxIncome = 999999999,
                TaxRate = 13,
                Description = "CPF Employer 13% (age 55-60)"
            },
            new TaxBracket
            {
                Id = 10,
                Country = "Singapore_Employee_60to65",
                MinIncome = 0,
                MaxIncome = 999999999,
                TaxRate = 9.5m,
                Description = "CPF Employee 9.5% (age 60-65)"
            },
            new TaxBracket
            {
                Id = 11,
                Country = "Singapore_Employer_60to65",
                MinIncome = 0,
                MaxIncome = 999999999,
                TaxRate = 9,
                Description = "CPF Employer 9% (age 60-65)"
            },
            new TaxBracket
            {
                Id = 12,
                Country = "Singapore_Employee_65above",
                MinIncome = 0,
                MaxIncome = 999999999,
                TaxRate = 7,
                Description = "CPF Employee 7% (age above 65)"
            },
            new TaxBracket
            {
                Id = 13,
                Country = "Singapore_Employer_65above",
                MinIncome = 0,
                MaxIncome = 999999999,
                TaxRate = 7.5m,
                Description = "CPF Employer 7.5% (age above 65)"
            }
        );
    }
}
