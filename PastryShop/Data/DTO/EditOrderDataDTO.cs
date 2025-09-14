using PastryShop.Enum;

namespace PastryShop.Data.DTO
{
    public class EditOrderDataDTO
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public DateTime? OrderDate { get; set; }
        public decimal? TotalPrice { get; set; }
        public OrderStatus? Status { get; set; }
    }
}