using EmployeeService.Data;
using EmployeeService.Models;
using EmployeeService.Repostories;
using Microsoft.EntityFrameworkCore;

namespace EmployeeService.Repositories;

public class DepartmentRepository : Repository<Department>, IDepartmentRepository
{
    public DepartmentRepository(AppDbContext db) : base(db) { }

    public async Task<bool> NameExistsAsync(string name) =>
        await _db.Departments.AnyAsync(d => d.Name == name);
}