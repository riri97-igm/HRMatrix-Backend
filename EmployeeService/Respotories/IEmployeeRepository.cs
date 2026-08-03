using EmployeeService.Models;

namespace EmployeeService.Repositories;

public interface IEmployeeRepository : IRepository<Employee>
{
    Task<Employee?> GetByUserIdAsync(int userId);
    Task<IEnumerable<Employee>> GetByManagerIdAsync(int managerId);
    Task<IEnumerable<Employee>> GetAllWithDepartmentAsync();
    Task<Employee?> GetByIdWithDepartmentAsync(int id);
    Task<bool> EmailExistsAsync(string email);
}