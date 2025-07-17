using ConstructoraClean.Domain.Entities;

namespace ConstructoraClean.Domain.Interfaces;

public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(int id);
    Task<IEnumerable<Project>> GetByRegionAsync(int regionId);
    Task<decimal> GetTotalCostAsync(int projectId, DateTime from, DateTime to);
    Task<IEnumerable<TopMaterial>> GetTopMaterialsAsync(int projectId, DateTime from, DateTime to, int limit = 10);
    Task<IEnumerable<MonthlyBreakdown>> GetMonthlyBreakdownAsync(int projectId, DateTime from, DateTime to);
}

// DTOs específicos para queries complejas
public record TopMaterial(string Material, decimal TotalCost);
public record MonthlyBreakdown(string Month, decimal TotalCost); 