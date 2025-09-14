using System.ComponentModel;

namespace PastryShop.Enum
{
    public enum ModificationType
    {
        [Description("Rok upotrebe")]
        ExpirationDateChange,

        [Description("Upozorenje")]
        WarningDaysChange,

        [Description("Status")]
        StatusChange
    }
}