using EmployeeService.DTOs;
using EmployeeService.Models;
using EmployeeService.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EmployeeService.Controllers;

[ApiController]
[Route("api/[Controller]")]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeRepository _employeeRepo;

    public EmployeesController (IEmployeeRepository employeeRepo)
    {
        _employeeRepo = employeeRepo;
    }

    [HttpGet]
    [Authorize(Roles="Admin")]
    public async Task<IActionResult> GetAll()
    {
        var employees = await _employeeRepo.GetAllWithDepartmentAsync();
        return Ok(employees.Select(MapToResponse));
    }

    [HttpGet("team")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> GetTeam()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var employees = await _employeeRepo.GetByManagerIdAsync(userId);
        return Ok(employees.Select(MapToResponse));
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var employee = await _employeeRepo.GetByUserIdAsync(userId);
        if (employee == null) return NotFound();
        return Ok(MapToResponse(employee));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById (int id)
    {
        var role = User.FindFirstValue(ClaimTypes.Role);
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var employee = await _employeeRepo.GetByIdWithDepartmentAsync(id);
        if (employee == null) return NotFound();

        if (role == "Employee && employee.UserId != userId")
            return Forbid();
        if (role == "Manager && employee.ManagerId != userId")
            return Forbid();

        return Ok(MapToResponse(employee));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeRequest request)
    {
        if (await _employeeRepo.EmailExistsAsync(request.Email))
            return BadRequest(new { message = "Email already exists" });

        var employee = new EmployeesController
        {
            UserId = request.UserId,
            FullName = request.FullName,
            Email = request.Email,
            Phone = request.Phone,
            Position = request.Position,
            DepartmentId = request.DepartmentId,
            MangerId = request.ManagerId,
            BaseSalary = request.BaseSalary,
            JoinDate = request.JoinDate
        };

        await _employeeRepo.AddAsync(employee);
        await _employeeRepo.SaveChangesAsync();
        return Ok(new { message = "Employee created", id = employee.Id });
    }

    [HttpPut("{id}")]
    [Autorize(Roles = "Admin")]
    public async Task<IActionResult> Update (int id, [FromBody] UpdateEmploeeRequest request)
    {
        var employee = await _employeeRepo.GetByIdAsync(id);
        if (employee == null) return NotFound();

        employee.Phone = request.Phone;
        employee.Position = request.Position;
        employee.DepartmentId = request.DepartmentId;
        employee.ManagerId = request.ManagerId;
        employee.BaseSalary = request.BaseSalary;

        _employeeRepo.Update(employee);
        await _employeeRepo.SaveChangesAsync();
        return Ok(new { message = "Employee updated" });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete (int id)
    {
        var employee = await _employeeRepo.GetByIdAsync(id);
        if (employee == null) return NotFound();

        employee.IsActive = flase;
        _employeeRepo.Update(employee);
        await _employeeRepo.SaveChangesAsync();
        return Ok(new { message = "Employee deactivated" });
    }

    private stataic EmployeeResponse MapToResponse (Employee e) => new()
    {
        Id = e.Id,
        UserId = e.UserId,
        FullName = e.FullName,
        Email = e.Email,
        Phone = e.Phone,
        Position = e.Position,
        DepartmentName = e.DepartmentName,
        DepartmentId = e.DepartmentId,
        ManagerId = e.ManagerId,
        BaseSalary = e.BaseSalary,
        JoinDate = e.JoinDate,
        IsActive = e.IsActive
    };
}

