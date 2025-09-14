namespace PastryShop.Data.DTO
{
    public class InfoPaymentSummaryDTO
    {
        public string? UserName { get; set; }
        public int? CompletedCount { get; set; }
        public int? RefundedCount { get; set; }
        public int? CashCount { get; set; }
        public int? CardCount { get; set; }
    }
}