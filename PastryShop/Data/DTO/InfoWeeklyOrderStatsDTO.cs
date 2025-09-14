namespace PastryShop.Data.DTO
{
    public class InfoWeeklyOrderStatsDTO
    {
        public DateTime WeekStart { get; set; }
        public int TotalOrders { get; set; }
        public int TotalItems { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}