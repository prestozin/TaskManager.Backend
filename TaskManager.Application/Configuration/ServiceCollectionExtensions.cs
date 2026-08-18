using TaskManager.Application.Interfaces;
using TaskManager.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace TaskManager.Application.Configuration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtService, JwtService>();

        return services;
    }
}
