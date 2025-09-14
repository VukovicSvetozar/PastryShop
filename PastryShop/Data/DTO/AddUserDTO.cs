using PastryShop.Enum;

namespace PastryShop.Data.DTO
{
    public class AddUserDTO
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? ImagePath { get; set; }
        public decimal Salary { get; set; }
        public UserType UserType { get; set; } = UserType.Manager;
    }
}