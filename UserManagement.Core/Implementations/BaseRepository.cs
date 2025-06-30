using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using UserManagement.Data.Entities;

namespace UserManagement.Core.Implementations;
public class BaseRepository<TEntity> : Interfaces.IBaseRepository<TEntity> where TEntity : BaseEntity
{
    private readonly UserManagementDbContext _context;

    public BaseRepository(UserManagementDbContext dbContext)
    {
        _context = dbContext;
    }
    public async Task<TEntity> GetByIdAsync(long id)
    {
        var output = await _context.Set<TEntity>().FindAsync(id);
        if (output == null)
        {
            throw new InvalidOperationException($"Entity of type {typeof(TEntity).Name} with ID {id} not found.");
        }
        return output;
    }
    public async Task<TEntity> AddAsync(TEntity entity)
    {
        await _context.Set<TEntity>().AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }
    public async Task DeleteAsync(long id)
    {
        var entityToDelete = await GetByIdAsync(id);

        if ( entityToDelete != null)
        {
            _context.Set<TEntity>().Remove(entityToDelete);
            await _context.SaveChangesAsync();
        }
       
    }
    public async Task<List<TEntity>> GetAllAsync()
    {
        var output = await _context.Set<TEntity>().ToListAsync();
        return output;
    }

    public async Task UpdateAsync(TEntity entity)
    {
        _context.Set<TEntity>().Update(entity);
        await _context.SaveChangesAsync();
    }
}
