namespace PastryShop.Model
{
    public class AccessoryProduct : Product
    {
        public string Material { get; set; } = string.Empty;
        public string? Dimensions { get; set; }
        public bool IsReusable { get; set; } = false;
    }
}