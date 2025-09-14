using PastryShop.Enum;

namespace PastryShop.Data.DTO
{
    public class EditProductProfileDTO
    {
        public int Id { get; set; }
        public ProductType ProductType { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public string? ImagePath { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public FoodType? FoodType { get; set; }
        public decimal? Weight { get; set; }
        public bool? IsPerishable { get; set; }
        public int? Calories { get; set; }
        public decimal? Volume { get; set; }
        public bool? IsAlcoholic { get; set; }
        public string? Material { get; set; }
        public string? Dimensions { get; set; }
        public bool? IsReusable { get; set; }
    }
}