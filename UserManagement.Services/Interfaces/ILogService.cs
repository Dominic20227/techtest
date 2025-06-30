using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagement.Common.LogsModels;
using UserManagement.Core.Interfaces;
using UserManagement.Data.Entities;

namespace UserManagement.Services.Interfaces;
public interface ILogService : IBaseService<ILogRepository, Logs>
{
    public Task<List<LogsModel>> GetByUserIdAsync(long userId);
}
