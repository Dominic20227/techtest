using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using UserManagement.Common.LogsModels;
using UserManagement.Core.Interfaces;
using UserManagement.Data.Entities;
using UserManagement.Services.Interfaces;

namespace UserManagement.Services.Implementations;
public class LogService : BaseService<ILogRepository, Logs>, ILogService
{
    private readonly ILogRepository _logRepository;
    private readonly IMapper _mapper;
    public LogService(ILogRepository logRepository, IMapper mapper) : base(logRepository, mapper)
    {
        _logRepository = logRepository;
        _mapper = mapper;
    }

    public async Task<List<LogsModel>> GetByUserIdAsync(long userId)
    {
        var entities = await _logRepository.GetByUserIdAsync(userId);
        var output = _mapper.Map<List<LogsModel>>(entities);
        return output;
    }
}

