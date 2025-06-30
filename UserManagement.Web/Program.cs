using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UserManagement.Data;
using UserManagement.Services.Mapping;
using Westwind.AspNetCore.Markdown;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<UserManagementDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("UserManagementConnectionString"));
});
// Add services to the container.

builder.Services.AddAutoMapper(typeof(MappingConfig));

builder.Services
    .AddDomainServices()
    .AddMarkdown()
    .AddControllersWithViews();
    



var app = builder.Build();

app.UseMarkdown();

app.UseHsts();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();
