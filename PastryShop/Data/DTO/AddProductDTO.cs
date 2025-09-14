using PastryShop.Enum;

namespace PastryShop.Data.DTO
{
    public class AddProductDTO
    {
        public string Name { get; set; } = String.Empty;
        public string Description { get; set; } = String.Empty;
        public decimal Price { get; set; }
        public string? ImagePath { get; set; }
        public ProductType ProductType { get; set; }
    }
}