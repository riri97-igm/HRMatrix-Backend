using Microsoft.EntityFrameworkCore;
using PayrollService.Data;
using PayrollService.Models;

namespace PayrollService.Repositories;

public class LoanRepository : Repository<EmployeeLoan>, ILoanRepository
{
    public LoanRepository(AppDbContext db) : base(db) { }

    public async Task<IEnumerable<EmployeeLoan>> GetByEmployeeIdAsync(int employeeId) =>
        await _db.EmployeeLoans
            .Where(l => l.EmployeeId == employeeId)
            .OrderByDescending(l => l.AppliedDate)
            .ToListAsync();

    public async Task<IEnumerable<EmployeeLoan>> GetActiveLoansAsync(int employeeId) =>
        await _db.EmployeeLoans
            .Where(l => l.EmployeeId == employeeId
                && l.Status == LoanStatus.Approved
                && !l.IsSettled)
            .ToListAsync();

    public async Task<IEnumerable<EmployeeLoan>> GetAllLoansAsync() =>
        await _db.EmployeeLoans
            .OrderByDescending(l => l.AppliedDate)
            .ToListAsync();

    public async Task<IEnumerable<EmployeeLoan>> GetPendingHRLoansAsync() =>
     await _db.EmployeeLoans
         .Where(l => l.Status == LoanStatus.ManagerApproved)
         .OrderByDescending(l => l.AppliedDate)
         .ToListAsync();

    public async Task<IEnumerable<EmployeeLoan>> GetPendingManagerLoansAsync(int managerId) =>
        await _db.EmployeeLoans
            .Where(l => l.Status == LoanStatus.Pending
                && l.ManagerId == managerId)
            .OrderByDescending(l => l.AppliedDate)
            .ToListAsync();

    public async Task<IEnumerable<EmployeeLoan>> GetPendingCFOLoansAsync() =>
       await _db.EmployeeLoans
           .Where(l => l.Status == LoanStatus.HRApproved)
           .OrderByDescending(l => l.AppliedDate)
           .ToListAsync();

    public async Task<bool> HasActiveLoanAsync(int employeeId) =>
        await _db.EmployeeLoans
            .AnyAsync(l => l.EmployeeId == employeeId
                && (l.Status == LoanStatus.Approved && !l.IsSettled
                    || l.Status == LoanStatus.Pending
                    || l.Status == LoanStatus.ManagerApproved
                    || l.Status == LoanStatus.HRApproved));
}