using PastryShop.Enum;

namespace PastryShop.Data.DTO
{
    public class AddPaymentDTO
    {
        public int UserId { get; set; }
        public int OrderId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public decimal AmountPaid { get; set; }
        public string? CardNumber { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Now;
    }
}