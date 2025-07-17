namespace ConstructoraClean.Api.DTOs
{
    public class RegionOverrunDto
    {
        public int ProjectId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Budget { get; set; }
        public decimal TotalCost { get; set; }
        public decimal? OverrunPct { get; set; }
    }
}
