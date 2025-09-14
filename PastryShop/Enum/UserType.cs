using System.ComponentModel;

namespace PastryShop.Enum
{
    public enum UserType
    {
        [Description("Menadžer")]
        Manager,

        [Description("Blagajnik")]
        Cashier
    }
}