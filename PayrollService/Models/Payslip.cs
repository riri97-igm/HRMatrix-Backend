namespace PayrollService.Models;

public class Payslip
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;   
    public int Month { get; set; }
    public int Year { get; set; }

    // Earnings 
    public decimal BasicSalary { get; set; }
    public decimal Allowances { get; set; }
    public decimal OvertimePay { get; set; }
    public decimal YearEndBonus { get; set; }
    public decimal ThirteenthMonth { get; set; }

    // Deductions
    public decimal Deduction { get; set; }
    public decimal LoanDeduction { get; set; }
    public decimal TaxDeduction { get; set; }
    public decimal SocialSecurity { get; set; }

    // Calculated 
    public decimal GrossSalary { get; set; }
    public decimal TotalDeductions { get; set; }
    public decimal NetSalary { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public int GeneratedByUserId { get; set; }

}

public class EmployeeLoan
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public decimal TotalLoanAmount { get; set; }
    public decimal MonthlyDeduction { get; set; }
    public decimal RemainingBalance { get; set; }
    public bool IsSettled { get; set; } = false;
    public DateTime StartDate { get; set; }
    public DateTime? SettledDate { get; set; }
    public string Notes { get; set; } = string.Empty;
}
