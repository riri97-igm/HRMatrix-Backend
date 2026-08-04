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