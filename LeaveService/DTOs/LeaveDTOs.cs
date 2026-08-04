using LeaveService.Models;
using System;

namespace LeaveService.DTOs;

public class CreateLeaveRequest
{
    public LeaveType LeaveType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string EmployeeName { get; set; } = string.Empty;

    // Needed to calculate service years for entitlement
    public DateTime JoinDate { get; set; }
}

public class ReviewLeaveRequest
{
    public bool IsApproved { get; set; }
    public string? Comment { get; set; }
}

public class LeaveRequestResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string LeaveType { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalDays { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? ReviewComment { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class LeaveBalanceResponse
{
    public int Year { get; set; }

    // Annual leave breakdown
    public int AnnualEntitlement { get; set; }
    public int CarryForward { get; set; }
    public int TotalAnnualAvailable { get; set; }
    public int AnnualUsed { get; set; }
    public int AnnualRemaining { get; set; }

    // Medical leave
    public int MedicalTotal { get; set; }
    public int MedicalUsed { get; set; }
    public int MedicalRemaining { get; set; }

    // Service info
    public int ServiceYears { get; set; }
}