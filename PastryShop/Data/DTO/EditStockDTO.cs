using PastryShop.Enum;

namespace PastryShop.Data.DTO
{
    public class EditStockDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? OrderId { get; set; }
        public int? Quantity { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int? ExpirationWarningDays { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsWarning { get; set; }
        public TransactionType TransactionType { get; set; }
    }
}