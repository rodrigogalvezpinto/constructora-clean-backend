namespace ConstructoraClean.Application.Queries;

public record GetRegionOverrunsQuery(
    int RegionId,
    int Limit
);

public record RegionOverrunResult(
    int ProjectId,
    string Name,
    decimal Budget,
    decimal TotalCost,
    decimal? OverrunPct
); 