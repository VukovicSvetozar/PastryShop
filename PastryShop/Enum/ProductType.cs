using System.ComponentModel;

namespace PastryShop.Enum
{
    public enum ProductType
    {
        [Description("Piće")]
        Drink,

        [Description("Hrana")]
        Food,

        [Description("Dodaci")]
        Accessory
    }
}