using PayrollService.DTOs;
using PayrollService.Models;
using PayrollService.Repositories;

namespace PayrollService.Services;

public class PayrollService : IPayrollService
{
    private readonly IPayrollRepository _payrollRepo;
    private readonly ILoanRepository _loanRepo;
    private readonly ITaxService _taxService;
    private readonly ICountryPolicyRepository _countryRepo;
    private readonly IHttpClientFactory _httpClientFactory;

    public PayrollService(
        IPayrollRepository payrollRepo,
        ILoanRepository loanRepo,
        ITaxService taxService,
        ICountryPolicyRepository countryRepo,
        IHttpClientFactory httpClientFactory)
    {
        _payrollRepo = payrollRepo;
        _loanRepo = loanRepo;
        _taxService = taxService;
        _countryRepo = countryRepo;
        _httpClientFactory = httpClientFactory;
    }

    // Generate Payslip
    public async Task<(bool Success, string Message, int? Id)> GeneratePayslipAsync(
        int generatedByUserId, GeneratePayslipRequest request)
    {
        // Check duplicate
        if (await _payrollRepo.PayslipExistsAsync(
            request.EmployeeId, request.Month, request.Year))
            return (false, "Payslip already generated for this month", null);

        // Check country policy exists
        var policy = await _countryRepo.GetByCountryCodeAsync(request.CountryCode);
        if (policy == null)
            return (false, $"Country policy not found for {request.CountryCode}", null);

        // 1. Calculate gross salary
        var grossSalary = request.BasicSalary
            + request.Allowance
            + request.OvertimePay
            + request.YearEndBonus
            + request.ThirteenthMonth;

        // 2. Auto fetch active loans
        var activeLoans = await _loanRepo.GetActiveLoansAsync(request.EmployeeId);
        decimal loanDeduction = 0;

        foreach (var loan in activeLoans)
        {
            var deductAmount = Math.Min(loan.MonthlyDeduction, loan.RemainingBalance);
            loanDeduction += deductAmount;
            loan.RemainingBalance -= deductAmount;

            if (loan.RemainingBalance <= 0)
            {
                loan.RemainingBalance = 0;
                loan.IsSettled = true;
                loan.SettledDate = DateTime.UtcNow;
            }
            _loanRepo.Update(loan);
        }
        // 3. Auto calculate unpaid leave deduction
        decimal unpaidLeaveDeduction = 0;
        try
        {
            var client = _httpClientFactory.CreateClient("LeaveService");

            // Get JWT token from current request to forward to LeaveService
            var response = await client.GetAsync(
                $"/api/leave/unpaid?employeeId={request.EmployeeId}&month={request.Month}&year={request.Year}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadFromJsonAsync<UnpaidLeaveResponse>();
                if (content != null && content.TotalUnpaidDays > 0)
                {
                    // Calculate daily rate based on working days in month
                    var daysInMonth = DateTime.DaysInMonth(request.Year, request.Month);
                    var workingDays = GetWorkingDays(request.Year, request.Month);
                    var dailyRate = request.BasicSalary / workingDays;
                    unpaidLeaveDeduction = Math.Round(dailyRate * content.TotalUnpaidDays, 2);
                }
            }
        }
        catch { /* ignore if leave service unavailable */ }

        // 3. Calculate tax/contributions dynamically
        decimal taxDeduction = 0;
        decimal socialSecurity = 0;
        decimal cpfEmployee = 0;
        decimal cpfEmployer = 0;

        if (policy.HasProgressiveTax)
        {
            var annualGross = grossSalary * 12;
            taxDeduction = await _taxService.CalculateTaxAsync(
                request.CountryCode, annualGross);
        }

        if (policy.HasAgeBased)
        {
            var age = request.EmployeeAge ?? 30;
            var (empContrib, erContrib) = await _taxService
                .CalculateAgeBasedContributionAsync(request.CountryCode, grossSalary, age);
            cpfEmployee = empContrib;   // stored for reference only
            cpfEmployer = erContrib;    // stored for reference only
                                        // CPF is NOT deducted from salary - employee pays directly to IRAS
        }
        else
        {
            socialSecurity = await _taxService.CalculateSocialContributionAsync(
                request.CountryCode, request.BasicSalary);
        }

        // 4. Calculate total deduction
        // Remove cpfEmployee from total deduction
        var totalDeduction = taxDeduction
            + socialSecurity
            // + cpfEmployee  ← REMOVE THIS
            + loanDeduction
            + request.OtherDeduction;

        // 5. Calculate net salary
        var netSalary = grossSalary - totalDeduction;

        // 6. Create payslip
        var payslip = new Payslip
        {
            EmployeeId = request.EmployeeId,
            EmployeeName = request.EmployeeName,
            DepartmentName = request.DepartmentName,
            Month = request.Month,
            Year = request.Year,
            Country = policy.CountryName,
            BasicSalary = request.BasicSalary,
            Allowance = request.Allowance,
            OvertimePay = request.OvertimePay,
            YearEndBonus = request.YearEndBonus,
            ThirteenthMonth = request.ThirteenthMonth,
            GrossSalary = grossSalary,
            TaxDeduction = taxDeduction,
            SocialSecurity = socialSecurity,
            Cpfemployee = cpfEmployee,
            Cpfemployer = cpfEmployer,
            LoanDeduction = loanDeduction,
            OtherDeduction = request.OtherDeduction,
            TotalDeduction = totalDeduction,
            NetSalary = netSalary,
            Notes = request.Notes,
            GeneratedByUserId = generatedByUserId
        };

        await _payrollRepo.AddAsync(payslip);
        await _payrollRepo.SaveChangesAsync();
        return (true, "Payslip generated successfully", payslip.Id);
    }

    // Get My Payslips
    public async Task<IEnumerable<PayslipResponse>> GetMyPayslipsAsync(int employeeId)
    {
        var payslips = await _payrollRepo.GetByEmployeeIdAsync(employeeId);
        return payslips.Select(MapToResponse);
    }

    // Get Last 3 Months 
    public async Task<IEnumerable<PayslipResponse>> GetMyRecentPayslipsAsync(int employeeId)
    {
        var allPayslips = await _payrollRepo.GetByEmployeeIdAsync(employeeId);
        var threeMonthsAgo = DateTime.UtcNow.AddMonths(-3);
        return allPayslips
            .Where(p => new DateTime(p.Year, p.Month, 1) >= threeMonthsAgo)
            .Select(MapToResponse);
    }

    // Get All Payslips 
    public async Task<IEnumerable<PayslipResponse>> GetAllPayslipsAsync(int? year, int? month)
    {
        var payslips = await _payrollRepo.GetAllFilteredAsync(year, month);
        return payslips.Select(MapToResponse);
    }

    // Get Payslip By Id
    public async Task<PayslipResponse?> GetPayslipByIdAsync(
        int id, int employeeId, string role)
    {
        var payslip = await _payrollRepo.GetByIdAsync(id);
        if (payslip == null) return null;
        if (role == "Employee" && payslip.EmployeeId != employeeId) return null;
        return MapToResponse(payslip);
    }

    // Apply Loan
    public async Task<(bool Success, string Message, int? Id)> ApplyLoanAsync(
        ApplyLoanRequest request)
    {
        // Check if employee already has active loan
        if (await _loanRepo.HasActiveLoanAsync(request.EmployeeId))
            return (false, "Employee already has an active or pending loan", null);

        // Validate loan type
        if (!Enum.TryParse<LoanType>(request.LoanType, out var loanType))
            return (false, "Invalid loan type", null);

        // Calculate monthly deduction (fixed 12 months)
        var monthlyDeduction = Math.Round(request.RequestedAmount / 12, 2);

        var loan = new EmployeeLoan
        {
            EmployeeId = request.EmployeeId,
            EmployeeName = request.EmployeeName,
            DepartmentName = request.DepartmentName,
            ManagerId = request.ManagerId,
            LoanType = loanType,
            RequestedAmount = request.RequestedAmount,
            TotalLoanAmount = request.RequestedAmount,
            MonthlyDeduction = monthlyDeduction,
            RemainingBalance = request.RequestedAmount,
            RepaymentMonths = 12,
            Purpose = request.Purpose,
            Status = LoanStatus.Pending,
            AppliedDate = DateTime.UtcNow
        };

        await _loanRepo.AddAsync(loan);
        await _loanRepo.SaveChangesAsync();
        return (true, "Loan application submitted successfully", loan.Id);
    }

    // Get My Loans 
    public async Task<IEnumerable<LoanResponse>> GetMyLoansAsync(int employeeId)
    {
        var loans = await _loanRepo.GetByEmployeeIdAsync(employeeId);
        return loans.Select(MapLoanToResponse);
    }

    // Get All Loans 
    public async Task<IEnumerable<LoanResponse>> GetAllLoansAsync()
    {
        var loans = await _loanRepo.GetAllLoansAsync();
        return loans.Select(MapLoanToResponse);
    }

    //  Get Loans By Employee 
    public async Task<IEnumerable<LoanResponse>> GetLoansByEmployeeIdAsync(int employeeId)
    {
        var loans = await _loanRepo.GetByEmployeeIdAsync(employeeId);
        return loans.Select(MapLoanToResponse);
    }

    // Get Pending HR Loans
    public async Task<IEnumerable<LoanResponse>> GetPendingHRLoansAsync()
    {
        var loans = await _loanRepo.GetPendingHRLoansAsync();
        return loans.Select(MapLoanToResponse);
    }

    //  Get Pending Manager Loans 
    public async Task<IEnumerable<LoanResponse>> GetPendingManagerLoansAsync(int managerId)
    {
        var loans = await _loanRepo.GetPendingManagerLoansAsync(managerId);
        return loans.Select(MapLoanToResponse);
    }

    //  Get Pending CFO Loans 
    public async Task<IEnumerable<LoanResponse>> GetPendingCFOLoansAsync()
    {
        var loans = await _loanRepo.GetPendingCFOLoansAsync();
        return loans.Select(MapLoanToResponse);
    }

    // Manager Approve Loan (FIRST) 
    public async Task<(bool Success, string Message)> ManagerApproveLoanAsync(
        int loanId, int approverId, ApproveLoanRequest request)
    {
        var loan = await _loanRepo.GetByIdAsync(loanId);
        if (loan == null) return (false, "Loan not found");
        if (loan.Status != LoanStatus.Pending)
            return (false, "Loan must be in Pending status");

        loan.Status = LoanStatus.ManagerApproved;
        loan.ManagerApprovedBy = approverId;
        loan.ManagerApprovedByName = request.ApproverName;
        loan.ManagerApprovedAt = DateTime.UtcNow;
        loan.ManagerComment = request.Comment;

        _loanRepo.Update(loan);
        await _loanRepo.SaveChangesAsync();
        return (true, "Loan approved by Manager — forwarded to HR");
    }

    // HR Approve Loan (SECOND) 
    public async Task<(bool Success, string Message)> HRApproveLoanAsync(
        int loanId, int approverId, ApproveLoanRequest request)
    {
        var loan = await _loanRepo.GetByIdAsync(loanId);
        if (loan == null) return (false, "Loan not found");
        if (loan.Status != LoanStatus.ManagerApproved)
            return (false, "Loan must be Manager approved first");

        loan.Status = LoanStatus.HRApproved;
        loan.HRApprovedBy = approverId;
        loan.HRApprovedByName = request.ApproverName;
        loan.HRApprovedAt = DateTime.UtcNow;
        loan.HRComment = request.Comment;

        _loanRepo.Update(loan);
        await _loanRepo.SaveChangesAsync();
        return (true, "Loan approved by HR — forwarded to CFO");
    }

    // CFO Approve Loan (THIRD - FINAL) 
    public async Task<(bool Success, string Message)> CFOApproveLoanAsync(
        int loanId, int approverId, ApproveLoanRequest request)
    {
        var loan = await _loanRepo.GetByIdAsync(loanId);
        if (loan == null) return (false, "Loan not found");
        if (loan.Status != LoanStatus.HRApproved)
            return (false, "Loan must be HR approved first");

        loan.Status = LoanStatus.Approved;
        loan.CFOApprovedBy = approverId;
        loan.CFOApprovedByName = request.ApproverName;
        loan.CFOApprovedAt = DateTime.UtcNow;
        loan.CFOComment = request.Comment;
        loan.StartDate = DateTime.UtcNow;

        _loanRepo.Update(loan);
        await _loanRepo.SaveChangesAsync();
        return (true, "Loan fully approved — loan is now active");
    }

    // Reject Loan
    public async Task<(bool Success, string Message)> RejectLoanAsync(
        int loanId, int rejectorId, RejectLoanRequest request)
    {
        var loan = await _loanRepo.GetByIdAsync(loanId);
        if (loan == null) return (false, "Loan not found");
        if (loan.Status == LoanStatus.Approved || loan.Status == LoanStatus.Settled)
            return (false, "Cannot reject an approved or settled loan");

        loan.Status = LoanStatus.Rejected;
        loan.RejectedByName = request.RejectedByName;
        loan.RejectionReason = request.Reason;
        loan.RejectedAt = DateTime.UtcNow;

        _loanRepo.Update(loan);
        await _loanRepo.SaveChangesAsync();
        return (true, "Loan rejected");
    }

    // Settle Loan 
    public async Task<(bool Success, string Message)> SettleLoanAsync(int loanId)
    {
        var loan = await _loanRepo.GetByIdAsync(loanId);
        if (loan == null) return (false, "Loan not found");
        if (loan.IsSettled) return (false, "Loan already settled");

        loan.IsSettled = true;
        loan.Status = LoanStatus.Settled;
        loan.RemainingBalance = 0;
        loan.SettledDate = DateTime.UtcNow;

        _loanRepo.Update(loan);
        await _loanRepo.SaveChangesAsync();
        return (true, "Loan settled successfully");
    }

    // Map to Response 
    private static PayslipResponse MapToResponse(Payslip p) => new()
    {
        Id = p.Id,
        EmployeeId = p.EmployeeId,
        EmployeeName = p.EmployeeName,
        DepartmentName = p.DepartmentName,
        Month = p.Month,
        Year = p.Year,
        Country = p.Country,
        BasicSalary = p.BasicSalary,
        Allowance = p.Allowance,
        OvertimePay = p.OvertimePay,
        YearEndBonus = p.YearEndBonus,
        ThirteenthMonth = p.ThirteenthMonth,
        GrossSalary = p.GrossSalary,
        TaxDeduction = p.TaxDeduction,
        SocialSecurity = p.SocialSecurity,
        CpfEmployee = p.Cpfemployee,
        CpfEmployer = p.Cpfemployer,
        LoanDeduction = p.LoanDeduction,
        OtherDeduction = p.OtherDeduction,
        TotalDeduction = p.TotalDeduction,
        NetSalary = p.NetSalary,
        Notes = p.Notes,
        GeneratedAt = p.GeneratedAt
    };

    private static LoanResponse MapLoanToResponse(EmployeeLoan l) => new()
    {
        Id = l.Id,
        EmployeeId = l.EmployeeId,
        EmployeeName = l.EmployeeName,
        DepartmentName = l.DepartmentName,
        ManagerId = l.ManagerId,
        LoanType = l.LoanType.ToString(),
        RequestedAmount = l.RequestedAmount,
        TotalLoanAmount = l.TotalLoanAmount,
        MonthlyDeduction = l.MonthlyDeduction,
        RemainingBalance = l.RemainingBalance,
        RepaymentMonths = l.RepaymentMonths,
        Purpose = l.Purpose,
        Status = l.Status.ToString(),
        IsSettled = l.IsSettled,
        AppliedDate = l.AppliedDate,
        StartDate = l.StartDate,
        SettledDate = l.SettledDate,
        HRApprovedByName = l.HRApprovedByName,
        HRApprovedAt = l.HRApprovedAt,
        HRComment = l.HRComment,
        ManagerApprovedByName = l.ManagerApprovedByName,
        ManagerApprovedAt = l.ManagerApprovedAt,
        ManagerComment = l.ManagerComment,
        CFOApprovedByName = l.CFOApprovedByName,
        CFOApprovedAt = l.CFOApprovedAt,
        CFOComment = l.CFOComment,
        RejectedByName = l.RejectedByName,
        RejectionReason = l.RejectionReason,
        RejectedAt = l.RejectedAt,
        Notes = l.Notes,
    };
    public class UnpaidLeaveResponse
    {
        public int EmployeeId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int TotalUnpaidDays { get; set; }
    }
    private int GetWorkingDays(int year, int month)
    {
        var days = 0;
        var daysInMonth = DateTime.DaysInMonth(year, month);
        for (int i = 1; i <= daysInMonth; i++)
        {
            var day = new DateTime(year, month, i).DayOfWeek;
            if (day != DayOfWeek.Saturday && day != DayOfWeek.Sunday)
                days++;
        }
        return days;
    }
}