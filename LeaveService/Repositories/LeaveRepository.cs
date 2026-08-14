using LeaveService.Data;
using LeaveService.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveService.Repositories;

public class LeaveRepository : Repository<LeaveRequest>, ILeaveRepository
{
    public LeaveRepository(AppDbContext db) : base(db) { }

    public async Task<IEnumerable<LeaveRequest>> GetByEmployeeIdAsync(int employeeId) =>
        await _db.LeaveRequests
            .Where(l => l.EmployeeId == employeeId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    public async Task<IEnumerable<LeaveRequest>> GetPendingByManagerAsync(int managerId) =>
        await _db.LeaveRequests
            .Where(l => l.Status == LeaveStatus.Pending && l.ManagerId == managerId)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    public async Task<IEnumerable<LeaveRequest>> GetByManagerAsync(int managerId) =>
    await _db.LeaveRequests
        .Where(l => l.ManagerId == managerId)
        .OrderByDescending(l => l.CreatedAt)
        .ToListAsync();

    public async Task<IEnumerable<LeaveRequest>> GetPendingAsync() =>
        await _db.LeaveRequests
            .Where(l => l.Status == LeaveStatus.Pending)
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();

    public async Task<IEnumerable<LeaveRequest>> GetAllOrderedAsync() =>
        await _db.LeaveRequests
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
}