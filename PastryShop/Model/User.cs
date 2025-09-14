using PastryShop.Enum;

namespace PastryShop.Model
{
    public abstract class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string? ImagePath { get; set; }
        public DateOnly HireDate { get; set; } = DateOnly.FromDateTime(DateTime.Now);
        public decimal Salary { get; set; }
        public DateTime? LastLogin { get; set; }
        public UserType UserType { get; set; } = UserType.Manager;
        public Theme Theme { get; set; } = Theme.Light;
        public Language Language { get; set; } = Language.Serbian;
        public bool IsActive { get; set; } = true;
        public bool ForcePasswordChange { get; set; } = false;
    }
}