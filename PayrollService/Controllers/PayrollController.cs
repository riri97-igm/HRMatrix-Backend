using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollService.DTOs;
using PayrollService.Services;
using System.Security.Claims;

namespace PayrollService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PayrollController : ControllerBase
{
    private readonly IPayrollService _payrollService;

    public PayrollController(IPayrollService payrollService)
    {
        _payrollService = payrollService;
    }

    // Admin generates payslip
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Generate([FromBody] GeneratePayslipRequest request)
    {
        var generatedByUserId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var (success, message, id) = await _payrollService
            .GeneratePayslipAsync(generatedByUserId, request);

        if (!success) return BadRequest(new { message });
        return Ok(new { message, id });
    }

    // Employee views own payslips
    [HttpGet("my")]
    public async Task<IActionResult> GetMyPayslips()
    {
        var employeeId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var payslips = await _payrollService.GetMyPayslipsAsync(employeeId);
        return Ok(payslips);
    }

    // Admin views all payslips
    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? year,
        [FromQuery] int? month)
    {
        var payslips = await _payrollService.GetAllPayslipsAsync(year, month);
        return Ok(payslips);
    }

    // View specific payslip
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var employeeId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var role = User.FindFirstValue(ClaimTypes.Role)!;

        var payslip = await _payrollService
            .GetPayslipByIdAsync(id, employeeId, role);

        if (payslip == null) return NotFound();
        return Ok(payslip);
    }

    // Employee views last 3 months payslips
    [HttpGet("my/recent")]
    public async Task<IActionResult> GetMyRecentPayslips()
    {
        var employeeId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var payslips = await _payrollService.GetMyRecentPayslipsAsync(employeeId);
        return Ok(payslips);
    }
}