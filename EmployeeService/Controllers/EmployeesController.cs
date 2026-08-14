using EmployeeService.DTOs;
using EmployeeService.Models;
using EmployeeService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EmployeeService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var employees = await _employeeService.GetAllEmployeesAsync();
        return Ok(employees);
    }

    [HttpGet("team")]
    [Authorize(Roles = "Manager")]
    public async Task<IActionResult> GetTeam()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        // Get manager's employee record first
        var manager = await _employeeService.GetMyProfileAsync(userId);
        if (manager == null) return NotFound();

        // Then get team by manager's employee ID
        var employees = await _employeeService.GetTeamAsync(manager.Id);
        return Ok(employees);
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var employee = await _employeeService.GetMyProfileAsync(userId);
        if (employee == null) return NotFound();
        return Ok(employee);
    }
    // Employee updates own personal info
    [HttpPut("me/personal")]
    [Authorize]
    public async Task<IActionResult> UpdatePersonalInfo([FromBody] UpdatePersonalInfoRequest request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _employeeService.UpdatePersonalInfoAsync(userId, request);
        if (!result.Success) return BadRequest(new { message = result.Message });
        return Ok(new { message = result.Message });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var role = User.FindFirstValue(ClaimTypes.Role)!;
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var employee = await _employeeService.GetEmployeeByIdAsync(id, userId, role);
        if (employee == null) return NotFound();
        return Ok(employee);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateEmployeeRequest request)
    {
        var (success, message, id) = await _employeeService.CreateEmployeeAsync(request);
        if (!success) return BadRequest(new { message });
        return Ok(new { message, id });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateEmployeeRequest request)
    {
        var (success, message) = await _employeeService.UpdateEmployeeAsync(id, request);
        if (!success) return NotFound(new { message });
        return Ok(new { message });
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id, [FromBody] DeactivateEmployeeRequest request)
    {
        var (success, message) = await _employeeService.DeactivateEmployeeAsync(
            id, request.Status, request.ResignationDate, request.Remarks);
        if (!success) return NotFound(new { message });
        return Ok(new { message });
    }
}