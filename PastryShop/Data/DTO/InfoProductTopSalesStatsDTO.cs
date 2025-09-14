using PastryShop.Enum;

namespace PastryShop.Data.DTO
{
    public class InfoProductTopSalesStatsDTO
    {
        public int Id { get; set; }
        public ProductType ProductType { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        public int TotalSold { get; set; }
    }
}