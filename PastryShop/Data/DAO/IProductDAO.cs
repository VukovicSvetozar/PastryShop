using PastryShop.Data.DTO;
using PastryShop.Model;

namespace PastryShop.Data.DAO
{
    public interface IProductDAO
    {
        Task<Product?> GetProductById(int productId);
        Task<List<Product>> GetAllProducts();
        Task<List<string>> GetAllProductNames();
        Task<List<Order>> GetRecentOrdersForUser(int userId);
        Task<Order?> GetOrderById(int orderId);
        Task<List<OrderItem>> GetOrderItemsByOrderId(int orderId);
        Task<Payment?> GetPaymentByOrderId(int orderId);
        Task<List<InfoProductTopSalesStatsDTO>> GetBestSellersProducts(int limit);
        Task<InfoUserOrderStatsDTO> GetUserOrderStats(int userId, DateTime date);
        Task<InfoOrderStatsDTO> GetOrderStats(DateTime? from, DateTime? to);
        Task<List<InfoWeeklyOrderStatsDTO>> GetUserWeeklyStats(int? userId, int numberOfWeeks = 7);
        Task<int> AddProduct(Product product);
        Task<int> AddOrder(Order order);
        Task<int> AddOrderItem(OrderItem orderItem);
        Task<int> AddPayment(Payment payment);
        Task<bool> UpdateProductData(Product product);
        Task<bool> UpdateProductProfile(Product product);
        Task<bool> UpdateProductAvailability(int productId, bool isAvailable);
        Task<bool> UpdatePayment(Payment payment);
        Task<bool> UpdateOrderData(Order order);

    }
}