namespace PastryShop.Model
{
    public class Stock
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int? ExpirationWarningDays { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsWarning { get; set; } = false;
        public DateTime AddedDate { get; set; }
    }
}