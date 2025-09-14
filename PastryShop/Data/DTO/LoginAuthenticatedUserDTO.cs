using PastryShop.Enum;

namespace PastryShop.Data.DTO
{
    public class LoginAuthenticatedUserDTO
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? ImagePath { get; set; }
        public UserType UserType { get; set; }
        public Theme Theme { get; set; }
        public Language Language { get; set; }
        public bool IsActive { get; set; }
        public bool ForcePasswordChange { get; set; } = false;
    }
}