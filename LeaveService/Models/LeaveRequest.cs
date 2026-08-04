using System;

namespace LeaveService.Models;

public enum LeaveStatus { Pending, Approved, Rejected }

public enum LeaveType
{
    Annual,
    Medical,
    Emergency,
    Unpaid,
    Hospital,
    Compensate
}

public class LeaveRequest
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public LeaveType LeaveType { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TotalDays { get; set; }
    public string Reason { get; set; } = string.Empty;
    public LeaveStatus Status { get; set; } = LeaveStatus.Pending;
    public int? ReviewedByUserId { get; set; }
    public string? ReviewComment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReviewedAt { get; set; }
}

public class LeaveBalance
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int Year { get; set; }
    public int AnnualTotal { get; set; } = 14;
    public int AnnualUsed { get; set; } = 0;
    public int MedicalTotal { get; set; } = 14;
    public int MedicalUsed { get; set; } = 0;
}