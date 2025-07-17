namespace ConstructoraClean.Domain.Entities
{
    public class Purchase
    {
        public long Id { get; set; }
        public int ProjectId { get; set; }
        public int MaterialId { get; set; }
        public int SupplierId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost { get; set; }
    }
}
