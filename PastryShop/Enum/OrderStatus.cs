using System.ComponentModel;

namespace PastryShop.Enum
{
    public enum OrderStatus
    {
        [Description("Završeno")]
        Completed,

        [Description("Poništeno")]
        Cancelled,

        [Description("Zadržano")]
        OnHold
    }
}