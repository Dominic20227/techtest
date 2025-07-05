using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using UserManagement.Data;
using UserManagement.Data.Entities;
using UserManagement.Services.Mapping;
using Westwind.AspNetCore.Markdown;


var builder = WebApplication.CreateBuilder(args);

// Detect if running in Azure
var isRunningInAzure = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_SITE_NAME")) ||
                       !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("AZURE_CLIENT_ID")) ||
                       builder.Environment.IsProduction();

// Check configuration override
var useInMemoryDatabase = builder.Configuration.GetValue<bool>("UseInMemoryDatabase", false);

// Use in-memory database if not in Azure or if explicitly configured
if (!isRunningInAzure || useInMemoryDatabase)
{
    builder.Services.AddDbContext<UserManagementDbContext>(options =>
    {
        options.UseInMemoryDatabase("UserManagementDb");
    });
}
else
{
    builder.Services.AddDbContext<UserManagementDbContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("UserManagementConnectionString"));
    });
}

builder.Services.AddAutoMapper(typeof(MappingConfig));

builder.Services
    .AddDomainServices()
    .AddMarkdown()
    .AddControllersWithViews();




var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<UserManagementDbContext>();

    if (dbContext.Database.IsRelational())
    {
        dbContext.Database.Migrate();
    }
    else
    {
        // Seed in-memory database with sample data
        SeedInMemoryDatabase(dbContext);
    }
}

app.UseMarkdown();

app.UseHsts();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();

static void SeedInMemoryDatabase(UserManagementDbContext context)
{
    // Check if data already exists
    if (context.Users.Any())
        return;

    var users = new[]
    {
        new User { Id = 1, Forename = "Peter", Surname = "Loew", DateOfBirth = new DateOnly(1993, 11, 12), Email = "ploew@example.com", IsActive = true },
        new User { Id = 2, Forename = "Benjamin Franklin", Surname = "Gates", DateOfBirth = new DateOnly(1985, 4, 17), Email = "bfgates@example.com", IsActive = true },
        new User { Id = 3, Forename = "Castor", Surname = "Troy", DateOfBirth = new DateOnly(1992, 8, 23), Email = "ctroy@example.com", IsActive = false },
        new User { Id = 4, Forename = "Memphis", Surname = "Raines", DateOfBirth = new DateOnly(1976, 2, 5), Email = "mraines@example.com", IsActive = true },
        new User { Id = 5, Forename = "Stanley", Surname = "Goodspeed", DateOfBirth = new DateOnly(2001, 11, 30), Email = "sgodspeed@example.com", IsActive = true },
        new User { Id = 6, Forename = "H.I.", Surname = "McDunnough", DateOfBirth = new DateOnly(1968, 7, 12), Email = "himcdunnough@example.com", IsActive = true },
        new User { Id = 7, Forename = "Cameron", Surname = "Poe", DateOfBirth = new DateOnly(1998, 5, 19), Email = "cpoe@example.com", IsActive = false },
        new User { Id = 8, Forename = "Edward", Surname = "Malus", DateOfBirth = new DateOnly(1954, 9, 3), Email = "emalus@example.com", IsActive = false },
        new User { Id = 9, Forename = "Damon", Surname = "Macready", DateOfBirth = new DateOnly(1989, 3, 27), Email = "dmacready@example.com", IsActive = false },
        new User { Id = 10, Forename = "Johnny", Surname = "Blaze", DateOfBirth = new DateOnly(1973, 6, 14), Email = "jblaze@example.com", IsActive = true },
        new User { Id = 11, Forename = "Robin", Surname = "Feld", DateOfBirth = new DateOnly(1982, 10, 8), Email = "rfeld@example.com", IsActive = true }
    };

    context.Users.AddRange(users);
    context.SaveChanges();
}
