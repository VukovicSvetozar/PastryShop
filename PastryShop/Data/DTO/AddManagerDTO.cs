namespace PastryShop.Data.DTO
{
    public class AddManagerDTO : AddUserDTO
    {
        public string Department { get; set; } = string.Empty;
    }
}