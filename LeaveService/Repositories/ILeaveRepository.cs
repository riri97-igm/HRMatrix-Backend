using LeaveService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeaveService.Repositories;

public interface ILeaveRepository : IRepository<LeaveRequest>
{
    Task<IEnumerable<LeaveRequest>> GetByUserIdAsync(int userId);
    Task<IEnumerable<LeaveRequest>> GetPendingAsync();
    Task<IEnumerable<LeaveRequest>> GetAllOrderedAsync();
}

public interface ILeaveBalanceRepository : IRepository<LeaveBalance>
{
    Task<LeaveBalance?> GetByUserAndYearAsync(int userId, int year);
}