namespace PastryShop.Data.DTO
{
    public class EditReduceStockDTO
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int OrderId { get; set; }
        public int QuantityToReduce { get; set; }
    }
}