using Microsoft.EntityFrameworkCore;
using PersonalLifeManager.Data;
using PersonalLifeManager.Models;

namespace PersonalLifeManager.Repositories;

public class Repository<T>(AppDbContext context) : IRepository<T>
    where T : class, ISoftDelete
{
    protected readonly DbSet<T> DbSet = context.Set<T>();

    public async Task<T?> GetByIdAsync(int id)
    {
        return await DbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await DbSet.ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await DbSet.AddAsync(entity);
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        DbSet.Remove(entity);
        await SaveChangesAsync();
    }

    public async Task SoftDeleteAsync(T entity)
    {
        entity.IsDeleted = true;
        await SaveChangesAsync();
    }

    public async Task RestoreAsync(T entity)
    {
        entity.IsDeleted = false;
        await SaveChangesAsync();
    }

    public async Task<IEnumerable<T>> GetAllIncludingDeletedAsync()
    {
        return await DbSet.IgnoreQueryFilters().ToListAsync();
    }
}