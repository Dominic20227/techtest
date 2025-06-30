using UserManagement.Data.Entities;

namespace UserManagement.Core.Interfaces;
public interface ILogRepository : IBaseRepository<Logs>
{
    public Task<List<Logs>> GetByUserIdAsync(long userId);
}
