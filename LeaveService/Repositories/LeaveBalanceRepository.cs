using LeaveService.Data;
using LeaveService.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LeaveService.Repositories;

public class LeaveBalanceRepository : Repository<LeaveBalance>, ILeaveBalanceRepository
{
    public LeaveBalanceRepository(AppDbContext db) : base(db) { }

    public async Task<LeaveBalance?> GetByUserAndYearAsync(int userId, int year) =>
        await _db.LeaveBalances
            .FirstOrDefaultAsync(b => b.UserId == userId && b.Year == year);
}