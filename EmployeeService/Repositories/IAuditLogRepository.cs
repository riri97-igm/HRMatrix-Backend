using EmployeeService.Models;

namespace EmployeeService.Repositories;

public interface IAuditLogRepository : IRepository<AuditLog>
{
    Task<IEnumerable<AuditLog>> GetRecentAsync(int count = 50);
    Task<IEnumerable<AuditLog>> GetByEntityTypeAsync(string entityType);
}