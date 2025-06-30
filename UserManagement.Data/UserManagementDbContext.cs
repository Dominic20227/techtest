using Microsoft.EntityFrameworkCore;
using UserManagement.Data.Entities;
using UserManagement.Data.Seeding;

namespace UserManagement.Data;

public class UserManagementDbContext : DbContext
{
    //public DataContext() => Database.EnsureCreated();

    public UserManagementDbContext(DbContextOptions<UserManagementDbContext> options) : base(options)
    {
    }
    public DbSet<User> Users { get; set; }
    public DbSet<Logs> Logs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new UserSeeding());
    }

}
