using EmployeeService.Models;

namespace EmployeeService.Repositories;

public interface IDepartmentRepository : IRepository<Department>
{
    Task<bool> NameExistsAsync(string name);
    Task<bool> HasEmployeesAsync(int departmentId);
}