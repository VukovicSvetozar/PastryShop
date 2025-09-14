using PastryShop.Enum;

namespace PastryShop.Model
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public decimal TotalPrice { get; set; }
        public OrderStatus Status { get; set; }
    }
}