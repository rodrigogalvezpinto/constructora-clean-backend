using ConstructoraClean.Application.Interfaces;
using ConstructoraClean.Application.Queries;
using ConstructoraClean.Domain.Interfaces;

namespace ConstructoraClean.Infrastructure.Services;

public class RegionOverrunService : IRegionOverrunService
{
    private readonly IRegionRepository _regionRepository;

    public RegionOverrunService(IRegionRepository regionRepository)
    {
        _regionRepository = regionRepository ?? throw new ArgumentNullException(nameof(regionRepository));
    }

    public async Task<IEnumerable<RegionOverrunResult>> GetTopOverrunsAsync(GetRegionOverrunsQuery query)
    {
        if (query == null)
            throw new ArgumentNullException(nameof(query));

        var overruns = await _regionRepository.GetTopOverrunsAsync(query.RegionId, query.Limit);
        
        return overruns.Select(o => new RegionOverrunResult(
            o.ProjectId,
            o.Name,
            o.Budget,
            o.TotalCost,
            o.OverrunPct
        ));
    }
} 