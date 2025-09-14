using PastryShop.Enum;

namespace PastryShop.Model
{
    public class FoodProduct : Product
    {
        public FoodType FoodType { get; set; }
        public decimal Weight { get; set; } = 0;
        public bool IsPerishable { get; set; } = true;
        public int? Calories { get; set; }
    }
}