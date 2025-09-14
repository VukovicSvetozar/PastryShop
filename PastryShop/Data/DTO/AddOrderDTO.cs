using PastryShop.Enum;

namespace PastryShop.Data.DTO
{
    public class AddOrderDTO
    {
        public int UserId { get; set; }
        public decimal TotalPrice { get; set; }
        public OrderStatus Status { get; set; }
    }
}