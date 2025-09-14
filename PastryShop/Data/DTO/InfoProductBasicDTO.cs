using PastryShop.Enum;

namespace PastryShop.Data.DTO
{
    public class InfoProductBasicDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ProductType ProductType { get; set; }
        public FoodType? FoodType { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; } = 0;
        public bool IsFeatured { get; set; } = false;
        public bool IsAvailable { get; set; } = true;
        public string? ImagePath { get; set; }
        public int Quantity { get; set; } = 0;
    }
}