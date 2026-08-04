using LeaveService.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeaveService.Services;

public interface ILeaveService
{
    Task<(bool Success, string Message, int? Id)> ApplyLeaveAsync(
        int userId, CreateLeaveRequest request);
    Task<IEnumerable<LeaveRequestResponse>> GetMyLeavesAsync(int userId);
    Task<IEnumerable<LeaveRequestResponse>> GetPendingLeavesAsync();
    Task<IEnumerable<LeaveRequestResponse>> GetAllLeavesAsync();
    Task<(bool Success, string Message)> ReviewLeaveAsync(
        int leaveId, int reviewerId, ReviewLeaveRequest request);
    Task<LeaveBalanceResponse> GetBalanceAsync(int userId, DateTime joinDate);
    int CalculateWorkingDays(DateTime startDate, DateTime endDate);
    int CalculateAnnualEntitlement(DateTime joinDate, int year);
    Task ProcessYearEndCarryForwardAsync(int userId, DateTime joinDate);
}