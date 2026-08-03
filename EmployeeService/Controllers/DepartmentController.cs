using EmployeeService.Models;
using EmployeeService.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DepartmentController : ControllerBase
{
    private readonly IDepartmentRepository _deptRepo;

    public DepartmentsController(IDepartmentRepository deptRepo)
    {
        _deptRepo = deptRepo;ျ
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var depts = await _deptRepo.GetAllAsync();
        return Ok(depts);
    }

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
}