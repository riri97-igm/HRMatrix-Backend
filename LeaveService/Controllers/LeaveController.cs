using LeaveService.DTOs;
using LeaveService.Models;
using LeaveService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace LeaveService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LeaveController : ControllerBase
{
    private readonly ILeaveService _leaveService;

    public LeaveController(ILeaveService leaveService)
    {
        _leaveService = leaveService;
    }

    [HttpPost]
    public async Task<IActionResult> Apply([FromBody] CreateLeaveRequest request)
    {
        var employeeId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var (success, message, id) = await _leaveService.ApplyLeaveAsync(employeeId, request);
        if (!success) return BadRequest(new { message });
        return Ok(new { message, id });
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyLeaves()
    {
        var employeeId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var leaves = await _leaveService.GetMyLeavesAsync(employeeId);
        return Ok(leaves);
    }

    [HttpGet("balance")]
    public async Task<IActionResult> GetBalance([FromQuery] DateTime joinDate)
    {
        var employeeId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var balance = await _leaveService.GetBalanceAsync(employeeId, joinDate);
        return Ok(balance);
    }

    [HttpGet("pending")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> GetPending()
    {
        var leaves = await _leaveService.GetPendingLeavesAsync();
        return Ok(leaves);
    }
    // Get approved unpaid leaves for employee in specific month/year
    [HttpGet("unpaid")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetApprovedUnpaidLeaves(
        [FromQuery] int employeeId,
        [FromQuery] int month,
        [FromQuery] int year)
    {
        var leaves = await _leaveService.GetMyLeavesAsync(employeeId);

        var unpaidLeaves = leaves.Where(l =>
            l.Status == "Approved" &&
            l.LeaveType == "Unpaid" &&
            DateTime.Parse(l.StartDate.ToString()).Month == month &&
            DateTime.Parse(l.StartDate.ToString()).Year == year)
            .ToList();

        var totalUnpaidDays = unpaidLeaves.Sum(l => l.TotalDays);

        return Ok(new
        {
            employeeId,
            month,
            year,
            totalUnpaidDays,
            leaves = unpaidLeaves
        });
    }

    [HttpPut("{id}/review")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> Review(int id, [FromBody] ReviewLeaveRequest request)
    {
        var reviewerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var (success, message) = await _leaveService.ReviewLeaveAsync(id, reviewerId, request);
        if (!success) return BadRequest(new { message });
        return Ok(new { message });
    }

    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var leaves = await _leaveService.GetAllLeavesAsync();
        return Ok(leaves);
    }
}