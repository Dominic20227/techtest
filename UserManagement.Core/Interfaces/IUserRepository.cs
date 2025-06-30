
using UserManagement.Data.Entities;

namespace UserManagement.Core.Interfaces;
public interface IUserRepository : IBaseRepository<User>
{
    public Task<List<User>?> GetByActiveStatusAsync(bool isActive);
    public Task<User?> GetUserWithLogsAsync(long userId);
}
