using PastryShop.Enum;

namespace PastryShop.Data.DTO
{
    public class AddFoodProductDTO : AddProductDTO
    {
        public FoodType? FoodType { get; set; }
        public decimal? Weight { get; set; }
        public bool? IsPerishable { get; set; }
        public int? Calories { get; set; }
    }
}