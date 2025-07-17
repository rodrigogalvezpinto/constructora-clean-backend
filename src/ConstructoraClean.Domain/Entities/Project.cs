namespace ConstructoraClean.Domain.Entities
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int RegionId { get; set; }
        public decimal Budget { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
