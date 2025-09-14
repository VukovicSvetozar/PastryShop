namespace PastryShop.Data.DTO
{
    public class InfoUserOrderStatsDTO
    {
        public int UserId { get; set; }
        public int TotalOrders { get; set; }
        public int TotalCompletedOrders { get; set; }
        public int TotalCancelledOrders { get; set; }
        public int TotalOnHoldOrders { get; set; }
        public int TotalItems { get; set; }
        public decimal TotalRevenue { get; set; }
        public int DailyOrders { get; set; }
        public int DailyCompletedOrders { get; set; }
        public int DailyCancelledOrders { get; set; }
        public int DailyOnHoldOrders { get; set; }
        public int DailyItems { get; set; }
        public decimal DailyRevenue { get; set; }
        public int DailyDrinkItems { get; set; }
        public int DailyFoodItems { get; set; }
        public int DailyAccessoryItems { get; set; }
    }
}