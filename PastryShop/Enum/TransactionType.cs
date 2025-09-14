using System.ComponentModel;

namespace PastryShop.Enum
{
    public enum TransactionType
    {
        [Description("Nabavljeno")]
        Addition,

        [Description("Prodato")]
        Sale,

        [Description("Vraćeno")]
        Return,

        [Description("Korekcija")]
        Adjustment
    }
}