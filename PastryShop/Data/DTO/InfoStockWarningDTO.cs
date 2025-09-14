namespace PastryShop.Data.DTO
{
    public class InfoStockWarningDTO
    {
        public int StockId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int? ExpirationWarningDays { get; set; }
    }
}