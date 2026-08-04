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

    public async Task<IEnumerable<LeaveRequest>> GetByUserIdAsync(int userId) =>
        await _db.LeaveRequests
            .Where(l => l.UserId == userId)
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