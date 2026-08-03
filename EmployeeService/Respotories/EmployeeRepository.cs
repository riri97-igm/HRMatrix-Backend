using EmployeeService.Data;
using EmployeeService.Models;
using EmployeeService.Repostories;
using Microsoft.EntityFrameworkCore;

namespace EmployeeService.Repositories;

public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(AppDbContext db) : base(db) { }

    public async Task<Employee?> GetByUserIdAsync(int userId) =>
        await _db.Employee
            .Include(e => e.Department)
            .FirstOrDefaultAsync(e => e.UserId == userId && e.IsActive);

    public async Task<IEnumerable<Employee>> GetByManagerIdAsync(int managerId) =>
        await _db.Employee
            .Include(e => e.Department)
            .Where(e => e.ManagerId == managerId && e.IsActive)
            .ToListAsync();

    public async Task<IEnumerable<Employee>> GetAllWithDepartmentAsync() =>
        await _db.Employee
            .Include(e => e.Department)
            .Where(e => e.IsActive)
            .ToListAsync();

    public async Task<Employee?> GetByIdWithDepartmentAsync(int id) =>
        await _db.Employee
            .Include(e => e.Department)
            .FirstOrDefaultAsync(e => e.Id == id);

    public async Task<bool> EmailExistsAsync(string email) =>
        await _db.Employee.AnyAsync(e => e.Email == email);
}