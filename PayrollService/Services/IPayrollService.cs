using PayrollService.DTOs;

namespace PayrollService.Services;

public interface IPayrollService
{
    // Payslip
    Task<(bool Success, string Message, int? Id)> GeneratePayslipAsync(
        int generatedByUserId, GeneratePayslipRequest request);
    Task<IEnumerable<PayslipResponse>> GetMyPayslipsAsync(int employeeId);
    Task<IEnumerable<PayslipResponse>> GetMyRecentPayslipsAsync(int employeeId);
    Task<IEnumerable<PayslipResponse>> GetAllPayslipsAsync(int? year, int? month);
    Task<PayslipResponse?> GetPayslipByIdAsync(int id, int employeeId, string role);

    // Loan
    Task<(bool Success, string Message, int? Id)> CreateLoanAsync(CreateLoanRequest request);
    Task<IEnumerable<LoanResponse>> GetLoansByEmployeeIdAsync(int employeeId);
    Task<IEnumerable<LoanResponse>> GetAllLoansAsync();
    Task<(bool Success, string Message)> SettleLoanAsync(int loanId);
}