using PastryShop.Enum;

namespace PastryShop.Data.DTO
{
    public class InfoUserBasicDTO
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public UserType UserType { get; set; }
        public bool IsActive { get; set; }
    }
}