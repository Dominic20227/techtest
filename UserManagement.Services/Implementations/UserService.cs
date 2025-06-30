using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using UserManagement.Common.Constants;
using UserManagement.Common.LogsModels;
using UserManagement.Common.UserModels;
using UserManagement.Core.Interfaces;
using UserManagement.Data.Entities;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Services.Implementations;
using UserManagement.Services.Interfaces;

namespace UserManagement.Services.Domain.Implementations;

public class UserService : BaseService<IUserRepository, User>, IUserService
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;
    private readonly ILogService _logService;
    public UserService(IUserRepository repository, IMapper mappingHelper, ILogService logService)
        : base(repository, mappingHelper)
    {
        _repository = repository;
        _mapper = mappingHelper;
        _logService = logService;
    }
    public async Task<List<UserModel>> GetByActiveStatus(bool isActive)
    {
        var userEntityList = await _repository.GetByActiveStatusAsync(isActive);

        if (userEntityList is null || userEntityList.Count == 0)
        {
            return new List<UserModel>();
        }
        else
        {
            var output = _mapper.Map<List<UserModel>>(userEntityList);
            return output;
        }

    }
    public async Task AddAsync(AddUserModel model)
    {
        if (model is null)
        {
            throw new ArgumentNullException(nameof(model), "Model cannot be null");
        }
        var userEntity = _mapper.Map<User>(model);
        userEntity.IsActive = true; // Assuming new users are active by default
        var newEntity = await _repository.AddAsync(userEntity);

        if (newEntity is null)
        {
            throw new InvalidOperationException("Failed to add user.");
        }

        await _logService.AddAsync<LogsModel>((new LogsModel
        {
            LogType = LogTypeConstants.UserAdded,
            LogMessage = $"User {userEntity.Forename} {userEntity.Surname} added successfully.",
            DateAndTime = DateTime.UtcNow,
            UserId = newEntity.Id
        }));
    }

    public async Task<UserModel?> GetByIdAsync(long id)
    {
        try
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return null;

            var output = _mapper.Map<UserModel>(entity);

            // Log the view action
            await _logService.AddAsync<LogsModel>(new LogsModel
            {
                LogType = LogTypeConstants.View,
                LogMessage = $"User {entity.Forename} {entity.Surname} was viewed.",
                DateAndTime = DateTime.UtcNow,
                UserId = entity.Id
            });

            return output;
        }
        catch (InvalidOperationException)
        {
            return null;
        }
    }

    public async Task UpdateAsync(UpdateUserModel model)
    {
        if (model is null)
        {
            throw new ArgumentNullException(nameof(model), "Model cannot be null");
        }

        var userEntity = _mapper.Map<User>(model);
        await _repository.UpdateAsync(userEntity);

        try
        {
            await _logService.AddAsync<LogsModel>(new LogsModel
            {
                LogType = LogTypeConstants.Update,
                LogMessage = $"User {userEntity.Forename} {userEntity.Surname} was updated.",
                DateAndTime = DateTime.UtcNow,
                UserId = userEntity.Id
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to log user update: {ex.Message}");
        }
    }

    public async Task<UserLogsModel?> GetUserAndLogsAsync(long id)
    {
        var user = await _repository.GetUserWithLogsAsync(id);

        if (user == null)
        {
            return null; 
        }
        var output = _mapper.Map<UserLogsModel>(user);
        return output;

    }
}
