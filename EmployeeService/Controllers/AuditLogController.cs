using EmployeeService.Models;
using EmployeeService.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EmployeeService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AuditLogController : ControllerBase
{
    private readonly IAuditLogRepository _auditRepo;

    public AuditLogController(IAuditLogRepository auditRepo)
    {
        _auditRepo = auditRepo;
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetRecent([FromQuery] int count = 50)
    {
        var logs = await _auditRepo.GetRecentAsync(count);
        return Ok(logs);
    }

    [HttpGet("entity/{entityType}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetByEntity(string entityType)
    {
        var logs = await _auditRepo.GetByEntityTypeAsync(entityType);
        return Ok(logs);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Add([FromBody] AuditLog log)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var userName = User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";
        var userRole = User.FindFirstValue(ClaimTypes.Role) ?? "Unknown";

        log.UserId = userId;
        log.UserName = userName;
        log.UserRole = userRole;
        log.CreatedAt = DateTime.UtcNow;
        log.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

        await _auditRepo.AddAsync(log);
        await _auditRepo.SaveChangesAsync();
        return Ok(new { message = "Audit log added" });
    }
}