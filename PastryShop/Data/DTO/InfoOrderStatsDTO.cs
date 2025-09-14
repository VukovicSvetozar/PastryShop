namespace PastryShop.Data.DTO
{
    public class InfoOrderStatsDTO
    {
        public int TotalOrders { get; set; }
        public int TotalCompletedOrders { get; set; }
        public int TotalCancelledOrders { get; set; }
        public int TotalOnHoldOrders { get; set; }
        public int TotalItems { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AvgRevenuePerOrder { get; set; }
        public double AvgItemsPerOrder { get; set; }
        public int DrinkItems { get; set; }
        public int FoodItems { get; set; }
        public int AccessoryItems { get; set; }
    }
}