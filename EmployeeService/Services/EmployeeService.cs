using EmployeeService.DTOs;
using EmployeeService.Models;
using EmployeeService.Repositories;

namespace EmployeeService.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepo;
    private readonly IDepartmentRepository _deptRepo;

    public EmployeeService(IEmployeeRepository employeeRepo, IDepartmentRepository deptRepo)
    {
        _employeeRepo = employeeRepo;
        _deptRepo = deptRepo;
    }

    public async Task<IEnumerable<EmployeeResponse>> GetAllEmployeesAsync()
    {
        var employees = await _employeeRepo.GetAllWithDepartmentAsync();
        return employees.Select(MapToResponse);
    }

    public async Task<EmployeeResponse?> GetMyProfileAsync(int userId)
    {
        var employee = await _employeeRepo.GetByUserIdAsync(userId);
        if (employee == null) return null;
        return MapToResponse(employee);
    }

    public async Task<IEnumerable<EmployeeResponse>> GetTeamAsync(int managerId)
    {
        var employees = await _employeeRepo.GetByManagerIdAsync(managerId);
        return employees.Select(MapToResponse);
    }

    public async Task<EmployeeResponse?> GetEmployeeByIdAsync(int id, int requestingUserId, string role)
    {
        var employee = await _employeeRepo.GetByIdWithDepartmentAsync(id);
        if (employee == null) return null;

        // Role-based access check
        if (role == "Employee" && employee.UserId != requestingUserId)
            return null;
        if (role == "Manager" && employee.ManagerId != requestingUserId
            && employee.UserId != requestingUserId)
            return null;

        return MapToResponse(employee);
    }

    public async Task<(bool Success, string Message, int? Id)> CreateEmployeeAsync(
        CreateEmployeeRequest request)
    {
        // Check duplicate email
        if (await _employeeRepo.EmailExistsAsync(request.Email))
            return (false, "Email already exists", null);

        var employee = new Employee
        {
            UserId = request.UserId,
            FullName = request.FullName,
            Email = request.Email,
            Phone = request.Phone,
            Position = request.Position,
            DepartmentId = request.DepartmentId,
            ManagerId = request.ManagerId,
            BaseSalary = request.BaseSalary,
            JoinDate = request.JoinDate
        };

        await _employeeRepo.AddAsync(employee);
        await _employeeRepo.SaveChangesAsync();
        return (true, "Employee created successfully", employee.Id);
    }

    public async Task<(bool Success, string Message)> UpdateEmployeeAsync(
        int id, UpdateEmployeeRequest request)
    {
        var employee = await _employeeRepo.GetByIdAsync(id);
        if (employee == null) return (false, "Employee not found");

        employee.Phone = request.Phone;
        employee.Position = request.Position;
        employee.DepartmentId = request.DepartmentId;
        employee.ManagerId = request.ManagerId;
        employee.BaseSalary = request.BaseSalary;

        _employeeRepo.Update(employee);
        await _employeeRepo.SaveChangesAsync();
        return (true, "Employee updated successfully");
    }

    public async Task<(bool Success, string Message)> DeactivateEmployeeAsync(int id)
    {
        var employee = await _employeeRepo.GetByIdAsync(id);
        if (employee == null) return (false, "Employee not found");

        employee.IsActive = false;
        _employeeRepo.Update(employee);
        await _employeeRepo.SaveChangesAsync();
        return (true, "Employee deactivated successfully");
    }

    private static EmployeeResponse MapToResponse(Employee e) => new()
    {
        Id = e.Id,
        UserId = e.UserId,
        FullName = e.FullName,
        Email = e.Email,
        Phone = e.Phone,
        Position = e.Position,
        DepartmentName = e.Department?.Name ?? "",
        DepartmentId = e.DepartmentId,
        ManagerId = e.ManagerId,
        BaseSalary = e.BaseSalary,
        JoinDate = e.JoinDate,
        IsActive = e.IsActive
    };
}