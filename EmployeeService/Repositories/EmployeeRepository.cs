using EmployeeService.Data;
using EmployeeService.Models;
using EmployeeService.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EmployeeService.Repositories;

public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(AppDbContext db) : base(db) { }

    public override async Task<Employee?> GetByIdAsync(int id) =>
        await _db.Employees
            .Include(e => e.Department)
            .FirstOrDefaultAsync(e => e.Id == id);

    public async Task<Employee?> GetByUserIdAsync(int userId) =>
        await _db.Employees
            .Include(e => e.Department)
            .FirstOrDefaultAsync(e => e.UserId == userId && e.IsActive);

    public async Task<IEnumerable<Employee>> GetByManagerIdAsync(int managerId) =>
        await _db.Employees
            .Include(e => e.Department)
            .Where(e => e.ManagerId == managerId && e.IsActive)
            .ToListAsync();

    public async Task<IEnumerable<Employee>> GetAllWithDepartmentAsync() =>
        await _db.Employees
            .Include(e => e.Department)
            .ToListAsync();

    public async Task<Employee?> GetByIdWithDepartmentAsync(int id) =>
        await _db.Employees
            .Include(e => e.Department)
            .FirstOrDefaultAsync(e => e.Id == id);

    public async Task<bool> EmailExistsAsync(string email) =>
        await _db.Employees.AnyAsync(e => e.Email == email);
}