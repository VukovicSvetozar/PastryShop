namespace PastryShop.Data.DTO
{
    public class AddCashierDTO : AddUserDTO
    {
        public int CashRegisterId { get; set; }
        public DateTime ShiftStart { get; set; } = DateTime.Now;
        public DateTime ShiftEnd { get; set; } = DateTime.Now.AddHours(8);
    }
}