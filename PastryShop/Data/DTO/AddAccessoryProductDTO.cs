namespace PastryShop.Data.DTO
{
    public class AddAccessoryProductDTO : AddProductDTO
    {
        public string Material { get; set; } = String.Empty;
        public string? Dimensions { get; set; }
        public bool? IsReusable { get; set; }
    }
}