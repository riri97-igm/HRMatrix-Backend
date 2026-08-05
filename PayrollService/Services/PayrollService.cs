using PayrollService.DTOs;
using PayrollService.Models;
using PayrollService.Repositories;

namespace PayrollService.Services;

public class PayrollService : IPayrollService
{
    private readonly IPayrollRepository _payrollRepo;
    private readonly ILoanRepository _loanRepo;
    private readonly ITaxService _taxService;

    public PayrollService(
        IPayrollRepository payrollRepo,
        ILoanRepository loanRepo,
        ITaxService taxService)
    {
        _payrollRepo = payrollRepo;
        _loanRepo = loanRepo;
        _taxService = taxService;
    }

    //  Generate Payslip 
    public async Task<(bool Success, string Message, int? Id)> GeneratePayslipAsync(
        int generatedByUserId, GeneratePayslipRequest request)
    {
        // Check duplicate
        if (await _payrollRepo.PayslipExistsAsync(
            request.EmployeeId, request.Month, request.Year))
            return (false, "Payslip already generated for this month", null);

        // 1. Calculate gross salary
        var grossSalary = request.BasicSalary
            + request.Allowance
            + request.OvertimePay
            + request.YearEndBonus
            + request.ThirteenthMonth;

        // 2. Auto fetch active loans and calculate total loan deduction
        var activeLoans = await _loanRepo.GetActiveLoansAsync(request.EmployeeId);
        decimal loanDeduction = 0;

        foreach (var loan in activeLoans)
        {
            // Deduct monthly amount or remaining balance whichever is less
            var deductAmount = Math.Min(loan.MonthlyDeduction, loan.RemainingBalance);
            loanDeduction += deductAmount;

            // Update loan remaining balance
            loan.RemainingBalance -= deductAmount;

            // Check if loan is fully settled
            if (loan.RemainingBalance <= 0)
            {
                loan.RemainingBalance = 0;
                loan.IsSettled = true;
                loan.SettledDate = DateTime.UtcNow;
            }

            _loanRepo.Update(loan);
        }

        // 3. Calculate tax/CPF based on country
        decimal taxDeduction = 0;
        decimal socialSecurity = 0;
        decimal cpfEmployee = 0;
        decimal cpfEmployer = 0;

        if (request.Country == "Myanmar")
        {
            // Annual gross for tax calculation
            var annualGross = grossSalary * 12;
            taxDeduction = _taxService.CalculateMyanmarTax(annualGross);
            socialSecurity = _taxService.CalculateSocialSecurity(request.BasicSalary);
        }
        else if (request.Country == "Singapore")
        {
            var age = request.EmployeeAge ?? 30; // default 30 if not provided
            cpfEmployee = _taxService.CalculateCpfEmployee(grossSalary, age);
            cpfEmployer = _taxService.CalculateCpfEmployer(grossSalary, age);
        }

        // 4. Calculate total deduction
        var totalDeduction = request.Country == "Myanmar"
            ? taxDeduction + socialSecurity + loanDeduction + request.OtherDeduction
            : cpfEmployee + loanDeduction + request.OtherDeduction;

        // 5. Calculate net salary
        var netSalary = grossSalary - totalDeduction;

        // 6. Create payslip
        var payslip = new Payslip
        {
            EmployeeId = request.EmployeeId,
            EmployeeName = request.EmployeeName,
            Month = request.Month,
            Year = request.Year,
            Country = request.Country,

            // Earnings
            BasicSalary = request.BasicSalary,
            Allowance = request.Allowance,
            OvertimePay = request.OvertimePay,
            YearEndBonus = request.YearEndBonus,
            ThirteenthMonth = request.ThirteenthMonth,
            GrossSalary = grossSalary,

            // Deductions
            TaxDeduction = taxDeduction,
            SocialSecurity = socialSecurity,
            Cpfemployee = cpfEmployee,
            Cpfemployer = cpfEmployer,
            LoanDeduction = loanDeduction,
            OtherDeduction = request.OtherDeduction,

            // Summary
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

        // Employee can only see own payslip
        if (role == "Employee" && payslip.EmployeeId != employeeId)
            return null;

        return MapToResponse(payslip);
    }

    // Get Last 3 Months Payslips 
    public async Task<IEnumerable<PayslipResponse>> GetMyRecentPayslipsAsync(int employeeId)
    {
        var allPayslips = await _payrollRepo.GetByEmployeeIdAsync(employeeId);

        // Get last 3 months based on current date
        var threeMonthsAgo = DateTime.UtcNow.AddMonths(-3);

        return allPayslips
            .Where(p => new DateTime(p.Year, p.Month, 1) >= threeMonthsAgo)
            .Select(MapToResponse);
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

    //  Get All Loans 
    public async Task<IEnumerable<LoanResponse>> GetAllLoansAsync()
    {
        var loans = await _loanRepo.GetAllLoansAsync();
        return loans.Select(MapLoanToResponse);
    }

    // Settle Loan Manually 
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
}