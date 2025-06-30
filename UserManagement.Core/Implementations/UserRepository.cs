using Microsoft.EntityFrameworkCore;
using UserManagement.Core.Interfaces;
using UserManagement.Data;
using UserManagement.Data.Entities;

namespace UserManagement.Core.Implementations;
public class UserRepository : BaseRepository<User>, IUserRepository
{
    private readonly UserManagementDbContext _context;
    public UserRepository(UserManagementDbContext dbContext) :base(dbContext)
    {
        _context = dbContext;
    }

    public async Task<List<User>?> GetByActiveStatusAsync(bool isActive)
    {
        var output = await _context.Users
            .Where(u => u.IsActive == isActive)
            .ToListAsync();

            return output;
    }

    public async Task<User?> GetUserWithLogsAsync(long id)
    {
        var output = await _context.Users
            .Where(u => u.Id == id).Include(x => x.Logs)
            .FirstOrDefaultAsync();

        return output;
    }
}
