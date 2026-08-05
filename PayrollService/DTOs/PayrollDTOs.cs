namespace PayrollService.DTOs;

public class GeneratePayslipRequest
{
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public int Month { get; set; }
    public int Year { get; set; }
    public string Country { get; set; } = "Myanmar"; //can change contry 

    //  Admin enters these
    public decimal BasicSalary { get; set; }
    public decimal Allowance { get; set; }
    public decimal OvertimePay { get; set; }
    public decimal YearEndBonus { get; set; }
    public decimal ThirteenthMonth { get; set; }
    public decimal OtherDeduction { get; set; }

    // Singapore CPF only 
    public int? EmployeeAge { get; set; }

    public string Notes { get; set; } = string.Empty;

    // System auto calculates 
    // LoanDeduction   → fetched from active loans
    // TaxDeduction    → progressive tax (Myanmar)
    // SocialSecurity  → 2% of BasicSalary (Myanmar)
    // CpfEmployee     → by age (Singapore)
    // CpfEmployer     → by age (Singapore)
}

public class PayslipResponse
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public int Month { get; set; }
    public int Year { get; set; }
    public string Country { get; set; } = string.Empty;

    // Earnings 
    public decimal BasicSalary { get; set; }
    public decimal Allowance { get; set; }
    public decimal OvertimePay { get; set; }
    public decimal YearEndBonus { get; set; }
    public decimal ThirteenthMonth { get; set; }
    public decimal GrossSalary { get; set; }

    // Deductions (Myanmar)
    public decimal TaxDeduction { get; set; }
    public decimal SocialSecurity { get; set; }

    // Deductions (Singapore) 
    public decimal CpfEmployee { get; set; }
    public decimal CpfEmployer { get; set; }

    // Deductions (Both) 
    public decimal LoanDeduction { get; set; }
    public decimal OtherDeduction { get; set; }

    // Summary 
    public decimal TotalDeduction { get; set; }
    public decimal NetSalary { get; set; }

    public string Notes { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
}

public class CreateLoanRequest
{
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public decimal TotalLoanAmount { get; set; }
    public decimal MonthlyDeduction { get; set; }
    public DateTime StartDate { get; set; }
    public string Notes { get; set; } = string.Empty;
}

public class LoanResponse
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public decimal TotalLoanAmount { get; set; }
    public decimal MonthlyDeduction { get; set; }
    public decimal RemainingBalance { get; set; }
    public bool IsSettled { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? SettledDate { get; set; }
    public string Notes { get; set; } = string.Empty;
}