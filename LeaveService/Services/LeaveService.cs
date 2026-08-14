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

    // Entitlement based on service years 
    public int CalculateAnnualEntitlement(DateTime joinDate, int year)
    {
        // Calculate service years at start of the given year
        var referenceDate = new DateTime(year, 1, 1);
        var serviceYears = (referenceDate - joinDate).Days / 365;

        return serviceYears switch
        {
            < 1 => 8,
            < 3 => 10,
            < 6 => 14,
            < 11 => 16,
            _ => 18
        };
    }

    // Calculate working days (exclude weekends) 
    public int CalculateWorkingDays(DateTime startDate, DateTime endDate)
    {
        int workingDays = 0;
        var current = startDate;

        while (current <= endDate)
        {
            if (current.DayOfWeek != DayOfWeek.Saturday &&
                current.DayOfWeek != DayOfWeek.Sunday)
                workingDays++;
            current = current.AddDays(1);
        }

        return workingDays;
    }

    // Get or create balance for a year 
    private async Task<LeaveBalance> GetOrCreateBalanceAsync(
        int employeeId, int year, DateTime joinDate)
    {
        var balance = await _balanceRepo.GetByEmployeeAndYearAsync(employeeId, year);
        if (balance != null) return balance;

        // Auto create with carry forward from previous year
        var entitlement = CalculateAnnualEntitlement(joinDate, year);
        var carryForward = 0;

        // Get previous year balance and calculate carry forward
        var prevBalance = await _balanceRepo.GetByEmployeeAndYearAsync(employeeId, year - 1);
        if (prevBalance != null)
        {
            var prevRemaining = prevBalance.AnnualTotal
                + prevBalance.CarryForward
                - prevBalance.AnnualUsed;
            carryForward = Math.Max(0, prevRemaining);
        }

        balance = new LeaveBalance
        {
            EmployeeId = employeeId,
            Year = year,
            AnnualTotal = entitlement,
            AnnualUsed = 0,
            CarryForward = carryForward,
            MedicalTotal = 14,
            MedicalUsed = 0
        };

        await _balanceRepo.AddAsync(balance);
        await _balanceRepo.SaveChangesAsync();
        return balance;
    }

    //  Apply for leave
    public async Task<(bool Success, string Message, int? Id)> ApplyLeaveAsync(
        int employeeId, CreateLeaveRequest request)
    {
        // 1. Validate dates
        if (request.StartDate < DateTime.Today)
            return (false, "Start date cannot be in the past", null);

        if (request.EndDate < request.StartDate)
            return (false, "End date cannot be before start date", null);

        // 2. Check overlapping leave
        var existingLeaves = await _leaveRepo.GetByEmployeeIdAsync(employeeId);
        var hasOverlap = existingLeaves.Any(l =>
            l.Status != LeaveStatus.Rejected &&
            l.StartDate <= request.EndDate &&
            l.EndDate >= request.StartDate);

        if (hasOverlap)
            return (false, "You already have a leave request for these dates", null);

        // 3. Calculate working days
        var workingDays = CalculateWorkingDays(request.StartDate, request.EndDate);
        if (workingDays == 0)
            return (false, "Selected dates fall on weekends only", null);

        // 4. Check balance
        var year = request.StartDate.Year;
        var balance = await GetOrCreateBalanceAsync(employeeId, year, request.JoinDate);

        if (request.LeaveType == LeaveType.Annual)
        {
            var totalAvailable = balance.AnnualTotal + balance.CarryForward - balance.AnnualUsed;
            if (workingDays > totalAvailable)
                return (false, $"Insufficient annual leave. Available: {totalAvailable} days", null);
        }
        else if (request.LeaveType == LeaveType.Medical)
        {
            var remaining = balance.MedicalTotal - balance.MedicalUsed;
            if (workingDays > remaining)
                return (false, $"Insufficient medical leave. Remaining: {remaining} days", null);
        }

        // 5. Create leave request
        var leave = new LeaveRequest
        {
            EmployeeId = employeeId,
            EmployeeName = request.EmployeeName,
            LeaveType = request.LeaveType,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            TotalDays = workingDays,
            Reason = request.Reason,
            ManagerId = request.ManagerId, // ← add this
            Status = LeaveStatus.Pending,
        };

        await _leaveRepo.AddAsync(leave);
        await _leaveRepo.SaveChangesAsync();
        return (true, $"Leave request submitted. Working days: {workingDays}", leave.Id);
    }

    //  Get balance with full breakdown 
    public async Task<LeaveBalanceResponse> GetBalanceAsync(int employeeId, DateTime joinDate)
    {
        var year = DateTime.UtcNow.Year;
        var balance = await GetOrCreateBalanceAsync(employeeId, year, joinDate);
        var serviceYears = (DateTime.Today - joinDate).Days / 365;

        return new LeaveBalanceResponse
        {
            Year = year,
            AnnualEntitlement = balance.AnnualTotal,
            CarryForward = balance.CarryForward,
            TotalAnnualAvailable = balance.AnnualTotal + balance.CarryForward,
            AnnualUsed = balance.AnnualUsed,
            AnnualRemaining = balance.AnnualTotal + balance.CarryForward - balance.AnnualUsed,
            MedicalTotal = balance.MedicalTotal,
            MedicalUsed = balance.MedicalUsed,
            MedicalRemaining = balance.MedicalTotal - balance.MedicalUsed,
            ServiceYears = serviceYears
        };
    }

    //  Year end carry forward processor 
    public async Task ProcessYearEndCarryForwardAsync(int employeeId, DateTime joinDate)
    {
        var currentYear = DateTime.UtcNow.Year;
        var nextYear = currentYear + 1;

        // Check if next year balance already exists
        var nextYearBalance = await _balanceRepo.GetByEmployeeAndYearAsync(employeeId, nextYear);
        if (nextYearBalance != null) return;

        // Get current year balance
        var currentBalance = await _balanceRepo.GetByEmployeeAndYearAsync(employeeId, currentYear);
        if (currentBalance == null) return;

        // Calculate remaining days to carry forward
        var remaining = currentBalance.AnnualTotal
            + currentBalance.CarryForward
            - currentBalance.AnnualUsed;
        var carryForward = Math.Max(0, remaining);

        // Calculate next year entitlement
        var nextYearEntitlement = CalculateAnnualEntitlement(joinDate, nextYear);

        // Create next year balance
        var newBalance = new LeaveBalance
        {
            EmployeeId = employeeId,
            Year = nextYear,
            AnnualTotal = nextYearEntitlement,
            AnnualUsed = 0,
            CarryForward = carryForward,
            MedicalTotal = 14,
            MedicalUsed = 0
        };

        await _balanceRepo.AddAsync(newBalance);
        await _balanceRepo.SaveChangesAsync();
    }
    public async Task<IEnumerable<LeaveRequestResponse>> GetLeavesByManagerAsync(int managerId)
    {
        var leaves = await _leaveRepo.GetByManagerAsync(managerId);
        return leaves.Select(MapToResponse);
    }

    //  Review leave
    public async Task<(bool Success, string Message)> ReviewLeaveAsync(
        int leaveId, int reviewerId, ReviewLeaveRequest request)
    {
        var leave = await _leaveRepo.GetByIdAsync(leaveId);
        if (leave == null) return (false, "Leave request not found");
        if (leave.Status != LeaveStatus.Pending)
            return (false, "Leave request already reviewed");

        leave.Status = request.IsApproved ? LeaveStatus.Approved : LeaveStatus.Rejected;
        leave.ReviewedByEmployeeId = reviewerId;
        leave.ReviewComment = request.Comment;
        leave.ReviewedAt = DateTime.UtcNow;

        if (request.IsApproved)
        {
            var balance = await _balanceRepo.GetByEmployeeAndYearAsync(
                leave.EmployeeId, leave.StartDate.Year);

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

    public async Task<IEnumerable<LeaveRequestResponse>> GetPendingLeavesByManagerAsync(int managerId)
    {
        var leaves = await _leaveRepo.GetPendingByManagerAsync(managerId);
        return leaves.Select(MapToResponse);
    }

    public async Task<IEnumerable<LeaveRequestResponse>> GetMyLeavesAsync(int employeeId)
    {
        var leaves = await _leaveRepo.GetByEmployeeIdAsync(employeeId);
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

    private static LeaveRequestResponse MapToResponse(LeaveRequest l) => new()
    {
        Id = l.Id,
        EmployeeId = l.EmployeeId,
        ManagerId = l.ManagerId,
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