using EmployeeService.Data;
using EmployeeService.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EmployeeService.Repostories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _db;
    protected readonly DbSet<T> _dbSet;

    public Repository(AppDbContext db)
    {
        _db = db;
        _dbSet = _db.Set<T>();
    }

    public async Task<T?> GetIdAsync(int id) =>
        await _dbSet.FindAsync(id);

    public async Task<IEnumerable<T>> GetAllAsync() =>
        await _dbSet.ToListAsync();

    public async Task AddAsync (T entity) =>
        await _dbSet.AddAsync(entity);

    public void Update (T entity) => 
        _dbSet.Update(entity);

    public void Delete(T entity) =>
        _dbSet.Remove(entity);

    public async Task SaveChangesAsync() =>
        await _db.SaveChangesAsync();
}