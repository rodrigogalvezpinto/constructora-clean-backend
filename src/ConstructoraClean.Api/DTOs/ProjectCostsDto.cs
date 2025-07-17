namespace ConstructoraClean.Api.DTOs
{
    public class ProjectCostsDto
    {
        public decimal TotalCost { get; set; }
        public List<TopMaterialDto> TopMaterials { get; set; } = new();
        public List<MonthlyBreakdownDto> MonthlyBreakdown { get; set; } = new();
    }

    public class TopMaterialDto
    {
        public string Material { get; set; } = null!;
        public decimal TotalCost { get; set; }
    }

    public class MonthlyBreakdownDto
    {
        public string Month { get; set; } = null!;
        public decimal TotalCost { get; set; }
    }
}
