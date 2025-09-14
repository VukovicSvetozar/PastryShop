using System.ComponentModel;

namespace PastryShop.Enum
{
    public enum FoodType
    {
        [Description("Torte")]
        Cake,

        [Description("Kolači")]
        Pastry,

        [Description("Slatkiši")]
        Sweet,

        [Description("Peciva")]
        Bakery
    }
}