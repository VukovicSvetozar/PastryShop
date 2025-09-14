using PastryShop.Enum;

namespace PastryShop.Data.DTO
{
    public class EditChangeUserTypeDTO
    {
        public int UserId { get; set; }
        public UserType NewUserType { get; set; }
        public string? Department { get; set; }
        public int? CashRegisterId { get; set; }
        public DateTime? ShiftStart { get; set; }
        public DateTime? ShiftEnd { get; set; }
    }
}