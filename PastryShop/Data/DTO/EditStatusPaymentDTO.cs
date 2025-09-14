using PastryShop.Enum;

namespace PastryShop.Data.DTO
{
    public class EditStatusPaymentDTO
    {
        public int OrderId { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
        public DateTime? PaymentDate { get; set; }
    }
}