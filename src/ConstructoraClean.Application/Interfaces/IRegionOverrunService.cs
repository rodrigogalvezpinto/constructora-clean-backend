using ConstructoraClean.Application.Queries;

namespace ConstructoraClean.Application.Interfaces;

public interface IRegionOverrunService
{
    Task<IEnumerable<RegionOverrunResult>> GetTopOverrunsAsync(GetRegionOverrunsQuery query);
} 