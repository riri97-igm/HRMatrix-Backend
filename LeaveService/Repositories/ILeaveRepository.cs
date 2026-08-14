using LeaveService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeaveService.Repositories;

public interface ILeaveRepository : IRepository<LeaveRequest>
{
    Task<IEnumerable<LeaveRequest>> GetByEmployeeIdAsync(int employeeId);
    Task<IEnumerable<LeaveRequest>> GetPendingAsync();
    Task<IEnumerable<LeaveRequest>> GetAllOrderedAsync();
    Task<IEnumerable<LeaveRequest>> GetPendingByManagerAsync(int managerId);
    Task<IEnumerable<LeaveRequest>> GetByManagerAsync(int managerId);
}

public interface ILeaveBalanceRepository : IRepository<LeaveBalance>
{
    Task<LeaveBalance?> GetByEmployeeAndYearAsync(int employeeId, int year);
}