namespace PastryShop.Model
{
    public class DrinkProduct : Product
    {
        public decimal? Volume { get; set; }
        public bool IsAlcoholic { get; set; } = false;
    }
}