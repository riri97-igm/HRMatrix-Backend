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
        return employees.Select(e => MapToResponse(e));
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
        return employees.Select(e => MapToResponse(e));
    }

    public async Task<EmployeeResponse?> GetEmployeeByIdAsync(int id, int requestingUserId, string role)
    {
        var employee = await _employeeRepo.GetByIdWithDepartmentAsync(id);
        if (employee == null) return null;

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
            JoinDate = request.JoinDate,
            Address = request.Address,
            Birthday = request.Birthday,
            EmergencyContactName = request.EmergencyContactName,
            EmergencyContactPhone = request.EmergencyContactPhone,
            EmergencyContactRelation = request.EmergencyContactRelation,
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
        employee.Address = request.Address;
        employee.Birthday = request.Birthday;
        employee.EmergencyContactName = request.EmergencyContactName;
        employee.EmergencyContactPhone = request.EmergencyContactPhone;
        employee.EmergencyContactRelation = request.EmergencyContactRelation;

        _employeeRepo.Update(employee);
        await _employeeRepo.SaveChangesAsync();
        return (true, "Employee updated successfully");
    }

    public async Task<(bool Success, string Message)> DeactivateEmployeeAsync(
        int id, string status, DateTime? resignationDate, string remarks)
    {
        var employee = await _employeeRepo.GetByIdAsync(id);
        if (employee == null) return (false, "Employee not found");

        employee.IsActive = false;
        employee.Status = status;
        employee.ResignationDate = resignationDate;
        employee.Remarks = remarks;

        _employeeRepo.Update(employee);
        await _employeeRepo.SaveChangesAsync();
        return (true, $"Employee {status.ToLower()} successfully");
    }

    public async Task<(bool Success, string Message)> UpdatePersonalInfoAsync(
        int userId, UpdatePersonalInfoRequest request)
    {
        var employee = await _employeeRepo.GetByUserIdAsync(userId);
        if (employee == null) return (false, "Employee not found");

        employee.Phone = request.Phone;
        employee.Address = request.Address;
        employee.Birthday = request.Birthday;
        employee.EmergencyContactName = request.EmergencyContactName;
        employee.EmergencyContactPhone = request.EmergencyContactPhone;
        employee.EmergencyContactRelation = request.EmergencyContactRelation;

        _employeeRepo.Update(employee);
        await _employeeRepo.SaveChangesAsync();
        return (true, "Personal info updated successfully");
    }

    private static EmployeeResponse MapToResponse(Employee e) => new()
    {
        Id = e.Id,
        UserId = e.UserId,
        FullName = e.FullName,
        Email = e.Email,
        Phone = e.Phone,
        Position = e.Position,
        DepartmentId = e.DepartmentId,
        DepartmentName = e.Department?.Name ?? string.Empty,
        ManagerId = e.ManagerId,
        ManagerName = string.Empty,
        BaseSalary = e.BaseSalary,
        JoinDate = e.JoinDate,
        IsActive = e.IsActive,
        Status = e.Status.ToString(),
        ResignationDate = e.ResignationDate,
        Remarks = e.Remarks,
        Address = e.Address,
        Birthday = e.Birthday,
        EmergencyContactName = e.EmergencyContactName,
        EmergencyContactPhone = e.EmergencyContactPhone,
        EmergencyContactRelation = e.EmergencyContactRelation,
    };
}