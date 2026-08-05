using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollService.DTOs;
using PayrollService.Services;
using System.Security.Claims;

namespace PayrollService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LoanController : ControllerBase
{
    private readonly IPayrollService _payrollService;

    public LoanController(IPayrollService payrollService)
    {
        _payrollService = payrollService;
    }

    // Admin registers a new loan
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateLoanRequest request)
    {
        var (success, message, id) = await _payrollService.CreateLoanAsync(request);
        if (!success) return BadRequest(new { message });
        return Ok(new { message, id });
    }

    // Employee views own loans
    [HttpGet("my")]
    public async Task<IActionResult> GetMyLoans()
    {
        var employeeId = int.Parse(
            User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var loans = await _payrollService.GetLoansByEmployeeIdAsync(employeeId);
        return Ok(loans);
    }

    // Admin views all loans
    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var loans = await _payrollService.GetAllLoansAsync();
        return Ok(loans);
    }

    // Admin manually settles a loan
    [HttpPut("{id}/settle")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Settle(int id)
    {
        var (success, message) = await _payrollService.SettleLoanAsync(id);
        if (!success) return BadRequest(new { message });
        return Ok(new { message });
    }
}