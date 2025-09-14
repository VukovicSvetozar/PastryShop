using System.Collections.ObjectModel;
using PastryShop.Data.DTO;

namespace PastryShop.Service
{
    public interface ICartService
    {
        public ObservableCollection<InfoProductBasicDTO> GetProducts();
        public void AddProduct(InfoProductBasicDTO product);
        public void RemoveProduct(InfoProductBasicDTO product);
        public void ClearCart();

    }
}