namespace PastryShop.Data.DTO
{
    public class AddDrinkProductDTO : AddProductDTO
    {
        public decimal? Volume { get; set; }
        public bool? IsAlcoholic { get; set; } = false;
    }
}