using EmployeeService.Data;
using EmployeeService.Models;
using EmployeeService.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EmployeeService.Repositories;

public class DepartmentRepository : Repository<Department>, IDepartmentRepository
{
    public DepartmentRepository(AppDbContext db) : base(db) { }

    public async Task<bool> NameExistsAsync(string name) =>
        await _db.Departments.AnyAsync(d => d.Name == name);

    public async Task<bool> HasEmployeesAsync(int departmentId) =>
        await _db.Employees.AnyAsync(e => e.DepartmentId == departmentId && e.IsActive);
}