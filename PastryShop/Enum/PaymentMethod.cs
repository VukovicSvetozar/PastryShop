using System.ComponentModel;

namespace PastryShop.Enum
{
    public enum PaymentMethod
    {
        [Description("Keš")]
        Cash,

        [Description("Kartica")]
        Card
    }
}