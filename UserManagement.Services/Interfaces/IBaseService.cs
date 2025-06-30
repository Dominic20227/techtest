
using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagement.Common.UserModels;
using UserManagement.Core.Interfaces;
using UserManagement.Data.Entities;

namespace UserManagement.Services.Interfaces;
public interface IBaseService<TRepository, TEntity>
    where TRepository : IBaseRepository<TEntity>
    where TEntity : BaseEntity
{
    public Task<List<TModel>> GetAllAsync<TModel>() where TModel : BaseModel;
    public Task<TModel?> GetByIdAsync<TModel>(long id) where TModel : BaseModel;
    public Task<TModel> AddAsync<TModel>(TModel model) where TModel : BaseModel;
    public Task UpdateAsync<TModel>(TModel model) where TModel : BaseModel;
    public Task DeleteAsync(long id);
}
