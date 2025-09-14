namespace PastryShop.Data.DTO
{
    public class InfoStockSummaryDTO
    {
        public int ProductId { get; set; }
        public int TotalQuantity { get; set; }
        public int TotalAdded { get; set; }
        public int TotalSold { get; set; }
        public int TotalModifications { get; set; }
    }
}