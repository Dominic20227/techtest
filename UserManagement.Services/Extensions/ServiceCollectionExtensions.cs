﻿using UserManagement.Core.Implementations;
using UserManagement.Core.Interfaces;
using UserManagement.Services.Domain.Implementations;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.Services.Implementations;
using UserManagement.Services.Interfaces;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<ILogRepository, LogRepository>();
        services.AddScoped<ILogService, LogService>();

        return services;
    }
    
}
