namespace EmployeeService.Models;

public enum NotificationType
{
    LeaveRequest,
    LeaveApproved,
    LeaveRejected,
    LoanRequest,
    LoanHRApproved,
    LoanManagerApproved,
    LoanApproved,
    LoanRejected,
    PayslipGenerated,
    General
}

public class Notification
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; }
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? ActionUrl { get; set; }
}