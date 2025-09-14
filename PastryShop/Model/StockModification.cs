using PastryShop.Enum;

namespace PastryShop.Model
{
    public class StockModification
    {
        public int Id { get; set; }
        public int StockId { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public string OldValue { get; set; } = string.Empty;
        public string NewValue { get; set; } = string.Empty;
        public DateTime ModificationDate { get; set; }
        public ModificationType ModificationType { get; set; }
    }
}