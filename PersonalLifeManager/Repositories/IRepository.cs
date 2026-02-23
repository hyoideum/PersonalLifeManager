using PersonalLifeManager.Models;

namespace PersonalLifeManager.Repositories;

public interface IRepository<T> where T : class, ISoftDelete
{
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task SaveChangesAsync();
    Task DeleteAsync(T entity);
    Task SoftDeleteAsync(T entity);
    Task RestoreAsync(T entity);
    Task<IEnumerable<T>> GetAllIncludingDeletedAsync();
}