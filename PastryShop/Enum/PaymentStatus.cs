using System.ComponentModel;

namespace PastryShop.Enum
{
    public enum PaymentStatus
    {
        [Description("Završeno")]
        Completed,

        [Description("Neuspješno")]
        Failed,

        [Description("Refundisano")]
        Refunded
    }
}