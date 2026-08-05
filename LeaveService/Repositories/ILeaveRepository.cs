using LeaveService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeaveService.Repositories;

public interface ILeaveRepository : IRepository<LeaveRequest>
{
    Task<IEnumerable<LeaveRequest>> GetByEmployeeIdAsync(int employeeId);
    Task<IEnumerable<LeaveRequest>> GetPendingAsync();
    Task<IEnumerable<LeaveRequest>> GetAllOrderedAsync();
}

public interface ILeaveBalanceRepository : IRepository<LeaveBalance>
{
    Task<LeaveBalance?> GetByEmployeeAndYearAsync(int employeeId, int year);
}