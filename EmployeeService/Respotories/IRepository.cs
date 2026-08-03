namespace EmployeeService.Repositories;

public interface IRepository<T> where T : class
{
    Task<T?> GetIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    void Update (T entity);
    void Delete (T entity);
    Task SaveChangesAsync();
}