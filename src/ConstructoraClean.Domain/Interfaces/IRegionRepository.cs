using ConstructoraClean.Domain.Entities;

namespace ConstructoraClean.Domain.Interfaces;

public interface IRegionRepository
{
    Task<Region?> GetByIdAsync(int id);
    Task<IEnumerable<RegionOverrun>> GetTopOverrunsAsync(int regionId, int limit);
}

// DTO específico para overruns por región
public record RegionOverrun(
    int ProjectId, 
    string Name, 
    decimal Budget, 
    decimal TotalCost, 
    decimal? OverrunPct); 