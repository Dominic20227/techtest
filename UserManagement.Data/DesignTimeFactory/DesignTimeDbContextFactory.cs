using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace UserManagement.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<UserManagementDbContext>
{
    public UserManagementDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<UserManagementDbContext>();

        optionsBuilder.UseSqlServer("Server=(localdb)\\ProjectModels;Database=UserManagementDb;Trusted_Connection=true;Encrypt=false;MultipleActiveResultSets=True");

        return new UserManagementDbContext(optionsBuilder.Options);
    }
}
