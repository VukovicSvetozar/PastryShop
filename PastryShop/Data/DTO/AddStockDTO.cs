namespace PastryShop.Data.DTO
{
    public class AddStockDTO
    {
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public int Quantity { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int? ExpirationWarningDays { get; set; }
    }
}