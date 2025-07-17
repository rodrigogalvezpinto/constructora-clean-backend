using ConstructoraClean.Domain.Entities;
using ConstructoraClean.Domain.Interfaces;
using ConstructoraClean.Infrastructure.Data;
using Dapper;

namespace ConstructoraClean.Infrastructure.Repositories;

public class RegionRepository : IRegionRepository
{
    private readonly DapperContext _context;

    public RegionRepository(DapperContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Region?> GetByIdAsync(int id)
    {
        using var connection = _context.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<Region>(
            "SELECT id, name FROM region WHERE id = @id",
            new { id });
    }

    public async Task<IEnumerable<RegionOverrun>> GetTopOverrunsAsync(int regionId, int limit)
    {
        using var connection = _context.CreateConnection();
        var sql = @"
            SELECT 
                p.id AS ProjectId,
                p.name AS Name,
                p.budget AS Budget,
                COALESCE(cost_summary.total_cost, 0) AS TotalCost,
                CASE 
                    WHEN p.budget > 0 THEN 
                        ROUND(((COALESCE(cost_summary.total_cost, 0) - p.budget) / p.budget * 100)::numeric, 2)
                    ELSE NULL 
                END AS OverrunPct
            FROM project p
            LEFT JOIN (
                SELECT project_id, SUM(total_cost) as total_cost
                FROM purchase
                GROUP BY project_id
            ) cost_summary ON cost_summary.project_id = p.id
            WHERE p.region_id = @regionId
            ORDER BY OverrunPct DESC NULLS LAST
            LIMIT @limit";

        return await connection.QueryAsync<RegionOverrun>(sql, new { regionId, limit });
    }
} 