using EmployeeService.Data;
using EmployeeService.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeService.Repositories;

public class AuditLogRepository : Repository<AuditLog>, IAuditLogRepository
{
    public AuditLogRepository(AppDbContext db) : base(db) { }

    public async Task<IEnumerable<AuditLog>> GetRecentAsync(int count = 50) =>
        await _db.AuditLogs
            .OrderByDescending(a => a.CreatedAt)
            .Take(count)
            .ToListAsync();

    public async Task<IEnumerable<AuditLog>> GetByEntityTypeAsync(string entityType) =>
        await _db.AuditLogs
            .Where(a => a.EntityType == entityType)
            .OrderByDescending(a => a.CreatedAt)
            .ToListAsync();
}