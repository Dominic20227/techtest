using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagement.Core.Interfaces;
using UserManagement.Data.Entities;
using UserManagement.Services.Interfaces;
using UserManagement.Common.Exceptions;
using UserManagement.Common.UserModels;
using AutoMapper;

namespace UserManagement.Services.Implementations;
public class BaseService<TRepository, TEntity> : IBaseService<TRepository, TEntity>
    where TRepository : IBaseRepository<TEntity>
    where TEntity : BaseEntity
{
    private readonly TRepository _repository;
    private readonly IMapper _mapper;

    public BaseService(TRepository repository, IMapper mappingHelper)
    {
        _repository = repository;
        _mapper = mappingHelper;
    }
    public async Task<TModel> AddAsync<TModel>(TModel model) where TModel : BaseModel
    {
        if (model is null)
        {
            throw new InvalidObjectException();
        }
        else
        {
            var entityToAdd = _mapper.Map<TModel, TEntity>(model);
            var addedEntity = await _repository.AddAsync(entityToAdd);

            var output = _mapper.Map<TEntity, TModel>(addedEntity);
            return output;
        }

    }
    public async Task DeleteAsync(long id)
    {
       await _repository.DeleteAsync(id);
        
    }
    public async Task<List<TModel>> GetAllAsync<TModel>() where TModel : BaseModel
    {
        var entities = await _repository.GetAllAsync();
        var output = _mapper.Map<List<TEntity>, List<TModel>>(entities);
        return output;
    }
    public async Task<TModel?> GetByIdAsync<TModel>(long id) where TModel : BaseModel
    {
        var entity = await _repository.GetByIdAsync(id);
        var output = _mapper.Map<TModel>(entity);
        return output;
    }

    public async Task UpdateAsync<TModel>(TModel model) where TModel : BaseModel
    {
        if (model is null)
        {
            throw new InvalidObjectException();
        }
        {
            var entity = _mapper.Map<TEntity>(model);
            await _repository.UpdateAsync(entity);
        }
    }
}

