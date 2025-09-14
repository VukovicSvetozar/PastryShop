using PastryShop.Enum;

namespace PastryShop.Data.DTO
{
    public class EditUserManagementDTO
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public UserType? UserType { get; set; }
        public decimal Salary { get; set; }
        public string? Department { get; set; }
        public int? CashRegisterId { get; set; }
        public DateTime? ShiftStart { get; set; }
        public DateTime? ShiftEnd { get; set; }
    }
}