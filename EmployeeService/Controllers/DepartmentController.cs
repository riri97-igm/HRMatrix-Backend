using EmployeeService.DTOs;
using EmployeeService.Models;
using EmployeeService.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentRepository _deptRepo;

    public DepartmentsController(IDepartmentRepository deptRepo)
    {
        _deptRepo = deptRepo;
    }

    // Get all departments
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var depts = await _deptRepo.GetAllAsync();
        return Ok(depts);
    }

    // Get department by ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var dept = await _deptRepo.GetByIdAsync(id);
        if (dept == null) return NotFound(new { message = "Department not found" });
        return Ok(dept);
    }

    // Create department
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] DepartmentRequest request)
    {
        if (await _deptRepo.NameExistsAsync(request.Name))
            return BadRequest(new { message = "Department already exists" });

        var dept = new Department { Name = request.Name };
        await _deptRepo.AddAsync(dept);
        await _deptRepo.SaveChangesAsync();
        return Ok(new { message = "Department created", id = dept.Id });
    }

    // Update department
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(int id, [FromBody] DepartmentRequest request)
    {
        var dept = await _deptRepo.GetByIdAsync(id);
        if (dept == null) return NotFound(new { message = "Department not found" });

        if (await _deptRepo.NameExistsAsync(request.Name) && dept.Name != request.Name)
            return BadRequest(new { message = "Department name already exists" });

        dept.Name = request.Name;
        _deptRepo.Update(dept);
        await _deptRepo.SaveChangesAsync();
        return Ok(new { message = "Department updated" });
    }

    // Delete department
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var dept = await _deptRepo.GetByIdAsync(id);
        if (dept == null) return NotFound(new { message = "Department not found" });

        // Check if department has employees
        if (await _deptRepo.HasEmployeesAsync(id))
            return BadRequest(new { message = "Cannot delete department with active employees" });

        _deptRepo.Delete(dept);
        await _deptRepo.SaveChangesAsync();
        return Ok(new { message = "Department deleted" });
    }
}