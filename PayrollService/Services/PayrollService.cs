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
            cpfEmployee = empContrib;
            cpfEmployer = erContrib;
        }
        else
        {
            socialSecurity = await _taxService.CalculateSocialContributionAsync(
                request.CountryCode, request.BasicSalary);
        }

        // 4. Calculate total deduction
        var totalDeduction = taxDeduction
            + socialSecurity
            + cpfEmployee
            + loanDeduction
            + request.OtherDeduction;

        // 5. Calculate net salary
        var netSalary = grossSalary - totalDeduction;

        // 6. Create payslip
        var payslip = new Payslip
        {
            EmployeeId = request.EmployeeId,
            EmployeeName = request.EmployeeName,
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

    // Create Loan 
    public async Task<(bool Success, string Message, int? Id)> CreateLoanAsync(
        CreateLoanRequest request)
    {
        if (request.TotalLoanAmount <= 0)
            return (false, "Loan amount must be greater than 0", null);
        if (request.MonthlyDeduction <= 0)
            return (false, "Monthly deduction must be greater than 0", null);
        if (request.MonthlyDeduction > request.TotalLoanAmount)
            return (false, "Monthly deduction cannot exceed total loan amount", null);

        var loan = new EmployeeLoan
        {
            EmployeeId = request.EmployeeId,
            EmployeeName = request.EmployeeName,
            TotalLoanAmount = request.TotalLoanAmount,
            MonthlyDeduction = request.MonthlyDeduction,
            RemainingBalance = request.TotalLoanAmount,
            StartDate = request.StartDate,
            Notes = request.Notes,
            IsSettled = false
        };

        await _loanRepo.AddAsync(loan);
        await _loanRepo.SaveChangesAsync();
        return (true, "Loan registered successfully", loan.Id);
    }

    // Get Loans By Employee
    public async Task<IEnumerable<LoanResponse>> GetLoansByEmployeeIdAsync(int employeeId)
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

    // Settle Loan 
    public async Task<(bool Success, string Message)> SettleLoanAsync(int loanId)
    {
        var loan = await _loanRepo.GetByIdAsync(loanId);
        if (loan == null) return (false, "Loan not found");
        if (loan.IsSettled) return (false, "Loan already settled");

        loan.IsSettled = true;
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
        TotalLoanAmount = l.TotalLoanAmount,
        MonthlyDeduction = l.MonthlyDeduction,
        RemainingBalance = l.RemainingBalance,
        IsSettled = l.IsSettled,
        StartDate = l.StartDate,
        SettledDate = l.SettledDate,
        Notes = l.Notes
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