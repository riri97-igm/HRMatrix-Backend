namespace PayrollService.DTOs;

public class GeneratePayslipRequest
{
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public int Month { get; set; }
    public int Year { get; set; }
    public string Country { get; set; } = "Myanmar"; //can change contry 
    public string CountryCode { get; set; } = "MM";

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
    public decimal UnpaidLeaveDeduction { get; set; }
    public int UnpaidLeaveDays { get; set; }
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

public class CountryPolicyResponse
{
    public int Id { get; set; }
    public string CountryCode { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public string FlagEmoji { get; set; } = string.Empty;
    public string SocialContributionLabel { get; set; } = string.Empty;
    public decimal SocialContributionEmployeeRate { get; set; }
    public decimal SocialContributionEmployerRate { get; set; }
    public bool HasProgressiveTax { get; set; }
    public bool HasAgeBased { get; set; }
    public bool IsActive { get; set; }
    public List<TaxBracketResponse> TaxBrackets { get; set; } = new();
    public List<AgeBracketResponse> AgeBrackets { get; set; } = new();
}

public class TaxBracketResponse
{
    public int Id { get; set; }
    public string Country { get; set; } = string.Empty;
    public decimal MinIncome { get; set; }
    public decimal MaxIncome { get; set; }
    public decimal TaxRate { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class AgeBracketResponse
{
    public int Id { get; set; }
    public string CountryCode { get; set; } = string.Empty;
    public int MinAge { get; set; }
    public int MaxAge { get; set; }
    public decimal EmployeeRate { get; set; }
    public decimal EmployerRate { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class CreateCountryPolicyRequest
{
    public string CountryCode { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public string FlagEmoji { get; set; } = string.Empty;
    public string SocialContributionLabel { get; set; } = string.Empty;
    public decimal SocialContributionEmployeeRate { get; set; }
    public decimal SocialContributionEmployerRate { get; set; }
    public bool HasProgressiveTax { get; set; }
    public bool HasAgeBased { get; set; }
}

public class CreateTaxBracketRequest
{
    public string CountryCode { get; set; } = string.Empty;
    public decimal MinIncome { get; set; }
    public decimal MaxIncome { get; set; }
    public decimal TaxRate { get; set; }
    public string Description { get; set; } = string.Empty;
}

public class CreateAgeBracketRequest
{
    public string CountryCode { get; set; } = string.Empty;
    public int MinAge { get; set; }
    public int MaxAge { get; set; }
    public decimal EmployeeRate { get; set; }
    public decimal EmployerRate { get; set; }
    public string Description { get; set; } = string.Empty;
}
