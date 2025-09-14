namespace PastryShop.Data.DTO
{
    public class EditProductDataDTO
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
        public bool IsFeatured { get; set; }
        public decimal Discount { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}