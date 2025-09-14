using PastryShop.Enum;

namespace PastryShop.Data.DTO
{
    public class InfoProductSalesSummaryDTO
    {
        public ProductType ProductType { get; set; }
        public int Count { get; set; }
        public int AvailableCount { get; set; }
        public int FeaturedCount { get; set; }
        public decimal TotalSalesAmount { get; set; }
        public decimal AverageSalesAmount { get; set; }
        public decimal FeaturedTotalSalesAmount { get; set; }
        public decimal FeaturedAverageSalesAmount { get; set; }
    }
}