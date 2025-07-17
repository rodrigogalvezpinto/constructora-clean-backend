using ConstructoraClean.Application.Queries;

namespace ConstructoraClean.Application.Interfaces;

public interface IProjectCostService
{
    Task<ProjectCostsResult?> GetProjectCostsAsync(GetProjectCostsQuery query);
} 