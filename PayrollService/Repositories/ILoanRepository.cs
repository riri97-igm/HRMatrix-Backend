using PayrollService.Models;

namespace PayrollService.Repositories;

public interface ILoanRepository : IRepository<EmployeeLoan>
{
    Task<IEnumerable<EmployeeLoan>> GetByEmployeeIdAsync(int employeeId);
    Task<IEnumerable<EmployeeLoan>> GetActiveLoansAsync(int employeeId);
    Task<IEnumerable<EmployeeLoan>> GetAllLoansAsync();
}