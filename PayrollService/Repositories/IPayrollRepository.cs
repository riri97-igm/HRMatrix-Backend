using PayrollService.Models;

namespace PayrollService.Repositories;

public interface IPayrollRepository : IRepository<Payslip>
{
    Task<IEnumerable<Payslip>> GetByEmployeeIdAsync(int employeeId);
    Task<IEnumerable<Payslip>> GetAllFilteredAsync(int? year, int? month);
    Task<bool> PayslipExistsAsync(int employeeId, int month, int year);
}