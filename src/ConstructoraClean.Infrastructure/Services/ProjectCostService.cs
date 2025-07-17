using ConstructoraClean.Application.Interfaces;
using ConstructoraClean.Application.Queries;
using ConstructoraClean.Domain.Interfaces;

namespace ConstructoraClean.Infrastructure.Services;

public class ProjectCostService : IProjectCostService
{
    private readonly IProjectRepository _projectRepository;

    public ProjectCostService(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
    }

    public async Task<ProjectCostsResult?> GetProjectCostsAsync(GetProjectCostsQuery query)
    {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        // Verificar que el proyecto existe
        var project = await _projectRepository.GetByIdAsync(query.ProjectId);
        if (project == null)
            return null;

        // Obtener datos en paralelo
        var totalCostTask = _projectRepository.GetTotalCostAsync(query.ProjectId, query.From, query.To);
        var topMaterialsTask = _projectRepository.GetTopMaterialsAsync(query.ProjectId, query.From, query.To);
        var monthlyBreakdownTask = _projectRepository.GetMonthlyBreakdownAsync(query.ProjectId, query.From, query.To);

        await Task.WhenAll(totalCostTask, topMaterialsTask, monthlyBreakdownTask);

        // Mapear a DTOs de Application
        var topMaterials = (await topMaterialsTask).Select(m => 
            new TopMaterialResult(m.Material, m.TotalCost));
        
        var monthlyBreakdown = (await monthlyBreakdownTask).Select(m => 
            new MonthlyBreakdownResult(m.Month, m.TotalCost));

        return new ProjectCostsResult(
            await totalCostTask,
            topMaterials,
            monthlyBreakdown
        );
    }
} 