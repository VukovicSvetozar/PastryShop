using PastryShop.Enum;

namespace PastryShop.Data.DTO
{
    public class InfoStockTransactionDTO
    {
        public int Id { get; set; }
        public int StockId { get; set; }
        public int ProductId { get; set; }
        public int? OrderId { get; set; }
        public int UserId { get; set; }
        public int QuantityChanged { get; set; }
        public DateTime TransactionDate { get; set; }
        public TransactionType TransactionType { get; set; }
    }
}