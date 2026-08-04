using LeaveService.DTOs;
using LeaveService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var (success, message, id) = await _leaveService.ApplyLeaveAsync(userId, request);
        if (!success) return BadRequest(new { message });
        return Ok(new { message, id });
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyLeaves()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var leaves = await _leaveService.GetMyLeavesAsync(userId);
        return Ok(leaves);
    }

    [HttpGet("balance")]
    public async Task<IActionResult> GetBalance()
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var balance = await _leaveService.GetBalanceAsync(userId);
        return Ok(balance);
    }

    [HttpGet("pending")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> GetPending()
    {
        var leaves = await _leaveService.GetPendingLeavesAsync();
        return Ok(leaves);
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