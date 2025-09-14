
namespace PastryShop.Data.DTO
{
    public class EditResetOrChangePasswordDTO
    {
        public int UserId { get; set; }
        public string NewPassword { get; set; } = string.Empty;
        public bool ForcePasswordChange { get; set; }
    }
}