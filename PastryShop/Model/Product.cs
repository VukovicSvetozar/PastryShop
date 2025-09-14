using PastryShop.Enum;

namespace PastryShop.Model
{
    public abstract class Product
    {
        public int Id { get; set; }
        public ProductType ProductType { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? ImagePath { get; set; }
        public bool IsAvailable { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }
        public bool IsFeatured { get; set; } = false;
        public decimal Discount { get; set; } = 0;
    }
}