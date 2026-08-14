using EmployeeService.DTOs;

namespace EmployeeService.Services;

public interface IEmployeeService
{
    Task<IEnumerable<EmployeeResponse>> GetAllEmployeesAsync();
    Task<EmployeeResponse?> GetEmployeeByIdAsync(int id, int requestingUserId, string role);
    Task<EmployeeResponse?> GetMyProfileAsync(int userId);
    Task<IEnumerable<EmployeeResponse>> GetTeamAsync(int managerId);
    Task<(bool Success, string Message, int? Id)> CreateEmployeeAsync(CreateEmployeeRequest request);
    Task<(bool Success, string Message)> UpdateEmployeeAsync(int id, UpdateEmployeeRequest request);
    Task<(bool Success, string Message)> DeactivateEmployeeAsync(
    int id, string status, DateTime? resignationDate, string remarks);
    Task<(bool Success, string Message)> UpdatePersonalInfoAsync(int userId, UpdatePersonalInfoRequest request);
}