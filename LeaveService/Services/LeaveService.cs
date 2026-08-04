using LeaveService.DTOs;
using LeaveService.Models;
using LeaveService.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveService.Services;

public class LeaveService : ILeaveService
{
    private readonly ILeaveRepository _leaveRepo;
    private readonly ILeaveBalanceRepository _balanceRepo;

    public LeaveService(ILeaveRepository leaveRepo, ILeaveBalanceRepository balanceRepo)
    {
        _leaveRepo = leaveRepo;
        _balanceRepo = balanceRepo;
    }

    public async Task<(bool Success, string Message, int? Id)> ApplyLeaveAsync(
        int userId, CreateLeaveRequest request)
    {
        // 1. Validate dates
        if (request.StartDate < DateTime.Today)
            return (false, "Start date cannot be in the past", null);

        if (request.EndDate < request.StartDate)
            return (false, "End date cannot be before start date", null);

        // 2. Check overlapping leave
        var existingLeaves = await _leaveRepo.GetByUserIdAsync(userId);
        var hasOverlap = existingLeaves.Any(l =>
            l.Status != LeaveStatus.Rejected &&
            l.StartDate <= request.EndDate &&
            l.EndDate >= request.StartDate);

        if (hasOverlap)
            return (false, "You already have a leave request for these dates", null);

        // 3. Calculate working days (exclude weekends)
        var workingDays = CalculateWorkingDays(request.StartDate, request.EndDate);

        if (workingDays == 0)
            return (false, "Selected dates fall on weekends only", null);

        // 4. Check leave balance
        var year = request.StartDate.Year;
        var balance = await _balanceRepo.GetByUserAndYearAsync(userId, year);
        if (balance == null)
        {
            balance = new LeaveBalance { UserId = userId, Year = year };
            await _balanceRepo.AddAsync(balance);
            await _balanceRepo.SaveChangesAsync();
        }

        if (request.LeaveType == LeaveType.Annual)
        {
            var remaining = balance.AnnualTotal - balance.AnnualUsed;
            if (workingDays > remaining)
                return (false, $"Insufficient annual leave balance. Remaining: {remaining} days", null);
        }
        else if (request.LeaveType == LeaveType.Medical)
        {
            var remaining = balance.MedicalTotal - balance.MedicalUsed;
            if (workingDays > remaining)
                return (false, $"Insufficient medical leave balance. Remaining: {remaining} days", null);
        }

        // 5. Create leave request
        var leave = new LeaveRequest
        {
            UserId = userId,
            EmployeeName = request.EmployeeName,
            LeaveType = request.LeaveType,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            TotalDays = workingDays,
            Reason = request.Reason,
            Status = LeaveStatus.Pending
        };

        await _leaveRepo.AddAsync(leave);
        await _leaveRepo.SaveChangesAsync();
        return (true, "Leave request submitted successfully", leave.Id);
    }

    public async Task<IEnumerable<LeaveRequestResponse>> GetMyLeavesAsync(int userId)
    {
        var leaves = await _leaveRepo.GetByUserIdAsync(userId);
        return leaves.Select(MapToResponse);
    }

    public async Task<IEnumerable<LeaveRequestResponse>> GetPendingLeavesAsync()
    {
        var leaves = await _leaveRepo.GetPendingAsync();
        return leaves.Select(MapToResponse);
    }

    public async Task<IEnumerable<LeaveRequestResponse>> GetAllLeavesAsync()
    {
        var leaves = await _leaveRepo.GetAllOrderedAsync();
        return leaves.Select(MapToResponse);
    }

    public async Task<(bool Success, string Message)> ReviewLeaveAsync(
        int leaveId, int reviewerId, ReviewLeaveRequest request)
    {
        var leave = await _leaveRepo.GetByIdAsync(leaveId);
        if (leave == null) return (false, "Leave request not found");
        if (leave.Status != LeaveStatus.Pending)
            return (false, "Leave request already reviewed");

        leave.Status = request.IsApproved ? LeaveStatus.Approved : LeaveStatus.Rejected;
        leave.ReviewedByUserId = reviewerId;
        leave.ReviewComment = request.Comment;
        leave.ReviewedAt = DateTime.UtcNow;

        // Deduct balance only when approved
        if (request.IsApproved)
        {
            var balance = await _balanceRepo.GetByUserAndYearAsync(
                leave.UserId, leave.StartDate.Year);

            if (balance != null)
            {
                if (leave.LeaveType == LeaveType.Annual)
                    balance.AnnualUsed += leave.TotalDays;
                else if (leave.LeaveType == LeaveType.Medical)
                    balance.MedicalUsed += leave.TotalDays;
                _balanceRepo.Update(balance);
            }
        }

        _leaveRepo.Update(leave);
        await _leaveRepo.SaveChangesAsync();
        return (true, $"Leave {(request.IsApproved ? "approved" : "rejected")} successfully");
    }

    public async Task<object> GetBalanceAsync(int userId)
    {
        var year = DateTime.UtcNow.Year;
        var balance = await _balanceRepo.GetByUserAndYearAsync(userId, year);

        if (balance == null)
        {
            balance = new LeaveBalance { UserId = userId, Year = year };
            await _balanceRepo.AddAsync(balance);
            await _balanceRepo.SaveChangesAsync();
        }

        return new
        {
            balance.AnnualTotal,
            balance.AnnualUsed,
            AnnualRemaining = balance.AnnualTotal - balance.AnnualUsed,
            balance.MedicalTotal,
            balance.MedicalUsed,
            MedicalRemaining = balance.MedicalTotal - balance.MedicalUsed
        };
    }

    // Calculate working days excluding weekends
    public int CalculateWorkingDays(DateTime startDate, DateTime endDate)
    {
        int workingDays = 0;
        var current = startDate;

        while (current <= endDate)
        {
            if (current.DayOfWeek != DayOfWeek.Saturday &&
                current.DayOfWeek != DayOfWeek.Sunday)
            {
                workingDays++;
            }
            current = current.AddDays(1);
        }

        return workingDays;
    }

    private static LeaveRequestResponse MapToResponse(LeaveRequest l) => new()
    {
        Id = l.Id,
        UserId = l.UserId,
        EmployeeName = l.EmployeeName,
        LeaveType = l.LeaveType.ToString(),
        StartDate = l.StartDate,
        EndDate = l.EndDate,
        TotalDays = l.TotalDays,
        Reason = l.Reason,
        Status = l.Status.ToString(),
        ReviewComment = l.ReviewComment,
        CreatedAt = l.CreatedAt
    };
}