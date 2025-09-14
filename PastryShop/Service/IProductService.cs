using PastryShop.Data.DTO;

namespace PastryShop.Service
{
    public interface IProductService
    {
        Task<List<InfoProductBasicDTO>> GetAllProductsBasicInfo();
        Task<InfoProductDetailsDTO?> GetProductDetailsById(int productId);
        Task<List<string>> GetAllProductNames();
        Task<EditProductDataDTO?> GetProductDataForEditById(int productId);
        Task<EditProductProfileDTO?> GetProductProfileForEditById(int productId);
        Task<List<InfoOrderDTO>> GetRecentOrdersForUser(int userId);
        Task<List<InfoOrderItemDTO>> GetOrderItemsByOrderId(int orderId);
        Task<List<InfoProductTopSalesStatsDTO>> GetBestSellersProducts(int limit);
        Task<InfoUserOrderStatsDTO> GetUserOrderStats(int userId, DateTime date);
        Task<InfoOrderStatsDTO> GetOrderStats(DateTime? from, DateTime? to);
        Task<List<InfoWeeklyOrderStatsDTO>> GetUserWeeklyStats(int? userId, int numberOfWeeks = 7);
        Task CreateProduct(AddProductDTO product);
        Task<int> CreateOrder(AddOrderDTO addOrder);
        Task<int> CreateOrderItem(AddOrderItemDTO addOrderItemDTO);
        Task<int> CreatePayment(AddPaymentDTO addPaymentDTO);
        Task<bool> EditProductData(EditProductDataDTO productDto);
        Task EditProductProfile(EditProductProfileDTO productDto);
        Task<bool> ChangeProductAvailability(int productId, bool isAvailable);
        Task<bool> EditStatusPayment(EditStatusPaymentDTO paymentDTO);
        Task<bool> EditOrderData(EditOrderDataDTO orderDTO);

    }
}