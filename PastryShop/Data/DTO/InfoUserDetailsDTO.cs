using PastryShop.Enum;

namespace PastryShop.Data.DTO
{
    public class InfoUserDetailsDTO
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? ImagePath { get; set; }
        public DateOnly HireDate { get; set; }
        public decimal Salary { get; set; }
        public DateTime? LastLogin { get; set; }
        public UserType UserType { get; set; }
        public string? Department { get; set; }
        public int? CashRegisterId { get; set; }
        public DateTime? ShiftStart { get; set; }
        public DateTime? ShiftEnd { get; set; }
    }
}