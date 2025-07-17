using Microsoft.Extensions.DependencyInjection;
using ConstructoraClean.Application.Interfaces;
using ConstructoraClean.Domain.Interfaces;
using ConstructoraClean.Infrastructure.Data;
using ConstructoraClean.Infrastructure.Repositories;
using ConstructoraClean.Infrastructure.Services;

namespace ConstructoraClean.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Database context
        services.AddScoped<DapperContext>();
        
        // Repositories
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<IRegionRepository, RegionRepository>();
        
        // Services
        services.AddScoped<IProjectCostService, ProjectCostService>();
        services.AddScoped<IRegionOverrunService, RegionOverrunService>();
        
        return services;
    }
} 