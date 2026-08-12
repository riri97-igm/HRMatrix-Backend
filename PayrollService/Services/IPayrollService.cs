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
    Task<(bool Success, string Message, int? Id)> ApplyLoanAsync(
        ApplyLoanRequest request);
    Task<IEnumerable<LoanResponse>> GetMyLoansAsync(int employeeId);
    Task<IEnumerable<LoanResponse>> GetAllLoansAsync();
    Task<IEnumerable<LoanResponse>> GetLoansByEmployeeIdAsync(int employeeId);
    Task<IEnumerable<LoanResponse>> GetPendingHRLoansAsync();
    Task<IEnumerable<LoanResponse>> GetPendingManagerLoansAsync(int managerId);
    Task<IEnumerable<LoanResponse>> GetPendingCFOLoansAsync();
    Task<(bool Success, string Message)> HRApproveLoanAsync(
        int loanId, int approverId, ApproveLoanRequest request);
    Task<(bool Success, string Message)> ManagerApproveLoanAsync(
        int loanId, int approverId, ApproveLoanRequest request);
    Task<(bool Success, string Message)> CFOApproveLoanAsync(
        int loanId, int approverId, ApproveLoanRequest request);
    Task<(bool Success, string Message)> RejectLoanAsync(
        int loanId, int rejectorId, RejectLoanRequest request);
    Task<(bool Success, string Message)> SettleLoanAsync(int loanId);
}