namespace PastryShop.Model
{
    public class Cashier : User
    {
        public int CashRegisterId { get; set; }
        public DateTime ShiftStart { get; set; }
        public DateTime ShiftEnd { get; set; }
    }
}