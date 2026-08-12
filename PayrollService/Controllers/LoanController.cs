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

    private int GetUserId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private string GetUserName() =>
        User.FindFirstValue(ClaimTypes.Name) ?? "Unknown";

    private string GetUserRole() =>
        User.FindFirstValue(ClaimTypes.Role) ?? "";

    // Employee - Apply for loan
    [HttpPost("apply")]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> Apply([FromBody] ApplyLoanRequest request)
    {
        var userId = GetUserId();
        request.EmployeeId = userId;
        var (success, message, id) = await _payrollService.ApplyLoanAsync(request);
        if (!success) return BadRequest(new { message });
        return Ok(new { message, id });
    }

    // Employee - Get my loans
    [HttpGet("my")]
    public async Task<IActionResult> GetMyLoans()
    {
        var userId = GetUserId();
        var loans = await _payrollService.GetMyLoansAsync(userId);
        return Ok(loans);
    }

    // Admin - Get all loans
    [HttpGet("all")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAll()
    {
        var loans = await _payrollService.GetAllLoansAsync();
        return Ok(loans);
    }

    // Admin - Get loans by employee
    [HttpGet("employee/{employeeId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetByEmployee(int employeeId)
    {
        var loans = await _payrollService.GetLoansByEmployeeIdAsync(employeeId);
        return Ok(loans);
    }

    // HR/Admin - Get pending HR loans
    [HttpGet("pending-hr")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetPendingHR()
    {
        var loans = await _payrollService.GetPendingHRLoansAsync();
        return Ok(loans);
    }

    // Manager - Get pending manager loans
    [HttpGet("pending-manager")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> GetPendingManager()
    {
        var userId = GetUserId();
        var loans = await _payrollService.GetPendingManagerLoansAsync(userId);
        return Ok(loans);
    }

    // CFO/Admin - Get pending CFO loans
    [HttpGet("pending-cfo")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetPendingCFO()
    {
        var loans = await _payrollService.GetPendingCFOLoansAsync();
        return Ok(loans);
    }

    // HR/Admin - Approve loan (HR stage)
    [HttpPut("{id}/hr-approve")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> HRApprove(int id, [FromBody] ApproveLoanRequest request)
    {
        request.ApproverName = GetUserName();
        var (success, message) = await _payrollService.HRApproveLoanAsync(
            id, GetUserId(), request);
        if (!success) return BadRequest(new { message });
        return Ok(new { message });
    }

    // Manager - Approve loan (Manager stage)
    [HttpPut("{id}/manager-approve")]
    [Authorize(Roles = "Manager,Admin")]
    public async Task<IActionResult> ManagerApprove(int id, [FromBody] ApproveLoanRequest request)
    {
        request.ApproverName = GetUserName();
        var (success, message) = await _payrollService.ManagerApproveLoanAsync(
            id, GetUserId(), request);
        if (!success) return BadRequest(new { message });
        return Ok(new { message });
    }

    // CFO/Admin - Final approve loan
    [HttpPut("{id}/cfo-approve")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CFOApprove(int id, [FromBody] ApproveLoanRequest request)
    {
        request.ApproverName = GetUserName();
        var (success, message) = await _payrollService.CFOApproveLoanAsync(
            id, GetUserId(), request);
        if (!success) return BadRequest(new { message });
        return Ok(new { message });
    }

    // HR/Admin/Manager - Reject loan
    [HttpPut("{id}/reject")]
    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Reject(int id, [FromBody] RejectLoanRequest request)
    {
        request.RejectedByName = GetUserName();
        var (success, message) = await _payrollService.RejectLoanAsync(
            id, GetUserId(), request);
        if (!success) return BadRequest(new { message });
        return Ok(new { message });
    }

    // Admin - Settle loan
    [HttpPut("{id}/settle")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Settle(int id)
    {
        var (success, message) = await _payrollService.SettleLoanAsync(id);
        if (!success) return BadRequest(new { message });
        return Ok(new { message });
    }
}