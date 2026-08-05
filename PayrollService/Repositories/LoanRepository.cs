using Microsoft.EntityFrameworkCore;
using PayrollService.Models;
using PayrollService.Properties.Data;

namespace PayrollService.Repositories;

public class LoanRepository : Repository<EmployeeLoan>, ILoanRepository
{
    public LoanRepository(AppDbContext db) : base(db) { }

    public async Task<IEnumerable<EmployeeLoan>> GetByEmployeeIdAsync(int employeeId) =>
        await _db.EmployeeLoans
            .Where(l => l.EmployeeId == employeeId)
            .OrderByDescending(l => l.StartDate)
            .ToListAsync();

    public async Task<IEnumerable<EmployeeLoan>> GetActiveLoansAsync(int employeeId) =>
        await _db.EmployeeLoans
            .Where(l => l.EmployeeId == employeeId && !l.IsSettled)
            .ToListAsync();

    public async Task<IEnumerable<EmployeeLoan>> GetAllLoansAsync() =>
        await _db.EmployeeLoans
            .OrderByDescending(l => l.StartDate)
            .ToListAsync();
}
