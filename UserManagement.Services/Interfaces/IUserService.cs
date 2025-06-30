using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagement.Common.UserModels;
using UserManagement.Core.Interfaces;
using UserManagement.Data.Entities;
using UserManagement.Services.Interfaces;


namespace UserManagement.Services.Domain.Interfaces;

public interface IUserService : IBaseService<IUserRepository, User>
{
    public Task<List<UserModel>> GetByActiveStatus(bool isActive);
    public Task<UserModel?> GetByIdAsync(long id);
    public Task AddAsync(AddUserModel model);
    public Task UpdateAsync(UpdateUserModel model);
    public Task<UserLogsModel?> GetUserAndLogsAsync(long id);
}
