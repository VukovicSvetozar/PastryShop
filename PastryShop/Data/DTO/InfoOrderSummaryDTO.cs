namespace PastryShop.Data.DTO
{
    public class InfoOrderSummaryDTO
    {
        public decimal TotalAmount { get; set; }
        public decimal AverageAmount { get; set; }
        public int CompletedCount { get; set; }
        public int CancelledCount { get; set; }
    }
}