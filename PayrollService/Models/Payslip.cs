namespace PayrollService.Models;

public class Payslip
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
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

    // Deductions (Myanamr)
    public decimal TaxDeduction { get; set; }
    public decimal SocialSecurity { get; set; }

    // Deduction (Singapore)
    public decimal Cpfemployee { get; set; }
    public decimal Cpfemployer { get; set; }

    // Deduction (Both Countries)
    public decimal LoanDeduction { get; set; }
    public decimal OtherDeduction { get; set; }

    // Summary 
    public decimal TotalDeduction { get; set; }
    public decimal NetSalary { get; set; }
    public string Notes { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    public int GeneratedByUserId { get; set; }
    public decimal UnpaidLeaveDeduction { get; set; }
    public int UnpaidLeaveDays { get; set; }

}

public enum LoanStatus
{
    Pending,
    HRApproved,
    ManagerApproved,
    Approved,
    Rejected,
    Settled
}

public enum LoanType
{
    Personal,
    Emergency,
    Education,
    Equipment,
    Medical,
    FestivalAdvance
}

public class EmployeeLoan
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public int? ManagerId { get; set; }

    // Loan Details
    public LoanType LoanType { get; set; } = LoanType.Personal;
    public string LoanTypeDisplay => LoanType.ToString();
    public decimal RequestedAmount { get; set; }
    public decimal TotalLoanAmount { get; set; }
    public decimal MonthlyDeduction { get; set; }
    public decimal RemainingBalance { get; set; }
    public int RepaymentMonths { get; set; } = 12;
    public string Purpose { get; set; } = string.Empty;

    // Status
    public LoanStatus Status { get; set; } = LoanStatus.Pending;
    public bool IsSettled { get; set; } = false;

    // Dates
    public DateTime AppliedDate { get; set; } = DateTime.UtcNow;
    public DateTime? StartDate { get; set; }
    public DateTime? SettledDate { get; set; }

    // HR Approval
    public int? HRApprovedBy { get; set; }
    public string HRApprovedByName { get; set; } = string.Empty;
    public DateTime? HRApprovedAt { get; set; }
    public string HRComment { get; set; } = string.Empty;

    // Manager Approval
    public int? ManagerApprovedBy { get; set; }
    public string ManagerApprovedByName { get; set; } = string.Empty;
    public DateTime? ManagerApprovedAt { get; set; }
    public string ManagerComment { get; set; } = string.Empty;

    // CFO Approval
    public int? CFOApprovedBy { get; set; }
    public string CFOApprovedByName { get; set; } = string.Empty;
    public DateTime? CFOApprovedAt { get; set; }
    public string CFOComment { get; set; } = string.Empty;

    // Rejection
    public string RejectedByName { get; set; } = string.Empty;
    public string RejectionReason { get; set; } = string.Empty;
    public DateTime? RejectedAt { get; set; }

    public string Notes { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class TaxBracket
{
    public int Id { get; set; }
    public string Country { get; set; } = string.Empty;
    public decimal MinIncome { get; set; }
    public decimal MaxIncome { get; set; }
    public decimal TaxRate { get; set; }
    public string Description {  get; set; } = string.Empty;
}

public class CountryPolicy
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
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class AgeBracket
{
    public int Id { get; set; }
    public string CountryCode { get; set; } = string.Empty;
    public int MinAge { get; set; }
    public int MaxAge { get; set; }
    public decimal EmployeeRate { get; set; }
    public decimal EmployerRate { get; set; }
    public string Description { get; set; } = string.Empty;
}