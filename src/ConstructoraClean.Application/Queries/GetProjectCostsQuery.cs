namespace ConstructoraClean.Application.Queries;

public record GetProjectCostsQuery(
    int ProjectId,
    DateTime From,
    DateTime To
);

public record ProjectCostsResult(
    decimal TotalCost,
    IEnumerable<TopMaterialResult> TopMaterials,
    IEnumerable<MonthlyBreakdownResult> MonthlyBreakdown
);

public record TopMaterialResult(string Material, decimal TotalCost);
public record MonthlyBreakdownResult(string Month, decimal TotalCost); 