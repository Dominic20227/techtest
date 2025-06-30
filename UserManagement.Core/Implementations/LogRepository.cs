using Microsoft.EntityFrameworkCore;
using UserManagement.Core.Interfaces;
using UserManagement.Data;
using UserManagement.Data.Entities;

namespace UserManagement.Core.Implementations;
public class LogRepository : BaseRepository<Logs>, ILogRepository
{
    private readonly UserManagementDbContext _context;
    public LogRepository(UserManagementDbContext dbContext) : base(dbContext)
    {
        _context = dbContext;
    }

    public async Task<List<Logs>> GetByUserIdAsync(long userId)
    {
        var output =  await _context.Logs.Where(log => log.UserId == userId).ToListAsync();
        return output;
    }
        
}
