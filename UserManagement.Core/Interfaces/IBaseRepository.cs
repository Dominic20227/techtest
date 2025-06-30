using UserManagement.Data.Entities;

namespace UserManagement.Core.Interfaces
{
    public interface IBaseRepository<TEntity> where TEntity : BaseEntity 
    {
        public Task<List<TEntity>> GetAllAsync();
        public Task<TEntity> GetByIdAsync(long id);
        public Task<TEntity> AddAsync(TEntity entity);
        public  Task UpdateAsync(TEntity entity);
        public  Task DeleteAsync(long id);
    }
}
