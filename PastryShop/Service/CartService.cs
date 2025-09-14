using System.Windows;
using System.Collections.ObjectModel;
using PastryShop.Data.DTO;

namespace PastryShop.Service
{
    public class CartService : ICartService
    {
        public ObservableCollection<InfoProductBasicDTO> CartProducts { get; }

        public CartService()
        {
            CartProducts = [];
        }

        public ObservableCollection<InfoProductBasicDTO> GetProducts()
        {
            return CartProducts;
        }

        public void AddProduct(InfoProductBasicDTO product)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(() => CartProducts.Add(product));
            }
            else
            {
                CartProducts.Add(product);
            }
        }

        public void RemoveProduct(InfoProductBasicDTO product)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(() => CartProducts.Remove(product));
            }
            else
            {
                CartProducts.Remove(product);
            }
        }

        public void ClearCart()
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(() => CartProducts.Clear());
            }
            else
            {
                CartProducts.Clear();
            }
        }

    }
}