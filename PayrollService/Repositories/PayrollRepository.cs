using Microsoft.EntityFrameworkCore;
using PayrollService.Models;
using PayrollService.Data;

namespace PayrollService.Repositories;

public class PayrollRepository : Repository<Payslip>, IPayrollRepository
{
    public PayrollRepository(AppDbContext db) : base(db) { }

    public async Task<IEnumerable<Payslip>> GetByEmployeeIdAsync(int employeeId) =>
        await _db.Payslips
            .Where(p => p.EmployeeId == employeeId)
            .OrderByDescending(p => p.Year)
            .ThenByDescending(p => p.Month)
            .ToListAsync();

    public async Task<IEnumerable<Payslip>> GetAllFilteredAsync(int? year, int? month)
    {
        var query = _db.Payslips.AsQueryable();
        if (year.HasValue) query = query.Where(p => p.Year == year);
        if (month.HasValue) query = query.Where(p => p.Month == month);
        return await query
            .OrderByDescending(p => p.Year)
            .ThenByDescending(p => p.Month)
            .ToListAsync();
    }

    public async Task<bool> PayslipExistsAsync(int employeeId, int month, int year) =>
        await _db.Payslips.AnyAsync(p =>
            p.EmployeeId == employeeId &&
            p.Month == month &&
            p.Year == year);
}