using ConstructoraClean.Domain.Entities;
using ConstructoraClean.Domain.Interfaces;
using ConstructoraClean.Infrastructure.Data;
using Dapper;

namespace ConstructoraClean.Infrastructure.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly DapperContext _context;

    public ProjectRepository(DapperContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Project?> GetByIdAsync(int id)
    {
        using var connection = _context.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<Project>(
            "SELECT id, name, region_id as RegionId, budget, start_date as StartDate, end_date as EndDate FROM project WHERE id = @id",
            new { id });
    }

    public async Task<IEnumerable<Project>> GetByRegionAsync(int regionId)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<Project>(
            "SELECT id, name, region_id as RegionId, budget, start_date as StartDate, end_date as EndDate FROM project WHERE region_id = @regionId",
            new { regionId });
    }

    public async Task<decimal> GetTotalCostAsync(int projectId, DateTime from, DateTime to)
    {
        using var connection = _context.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<decimal>(
            "SELECT COALESCE(SUM(total_cost),0) FROM purchase WHERE project_id = @projectId AND purchase_date BETWEEN @from AND @to",
            new { projectId, from, to });
    }

    public async Task<IEnumerable<TopMaterial>> GetTopMaterialsAsync(int projectId, DateTime from, DateTime to, int limit = 10)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<TopMaterial>(
            @"SELECT m.name AS Material, SUM(p.total_cost) AS TotalCost
               FROM purchase p
               JOIN material m ON m.id = p.material_id
              WHERE p.project_id = @projectId AND p.purchase_date BETWEEN @from AND @to
              GROUP BY m.name
              ORDER BY TotalCost DESC
              LIMIT @limit",
            new { projectId, from, to, limit });
    }

    public async Task<IEnumerable<MonthlyBreakdown>> GetMonthlyBreakdownAsync(int projectId, DateTime from, DateTime to)
    {
        using var connection = _context.CreateConnection();
        return await connection.QueryAsync<MonthlyBreakdown>(
            @"SELECT to_char(purchase_date, 'YYYY-MM') AS Month, SUM(total_cost) AS TotalCost
              FROM purchase
             WHERE project_id = @projectId AND purchase_date BETWEEN @from AND @to
             GROUP BY Month
             ORDER BY Month",
            new { projectId, from, to });
    }
} 