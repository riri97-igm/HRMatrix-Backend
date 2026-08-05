using LeaveService.Data;
using LeaveService.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace LeaveService.Repositories;

public class LeaveBalanceRepository : Repository<LeaveBalance>, ILeaveBalanceRepository
{
    public LeaveBalanceRepository(AppDbContext db) : base(db) { }

    public async Task<LeaveBalance?> GetByEmployeeAndYearAsync(int employeeId, int year) =>
        await _db.LeaveBalances
            .FirstOrDefaultAsync(b => b.EmployeeId == employeeId && b.Year == year);
}