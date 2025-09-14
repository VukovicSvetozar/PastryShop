using PastryShop.Data.DTO;

namespace PastryShop.Service
{
    public interface IStockService
    {
        Task<List<InfoStockDTO>> GetAllStocksByProductId(int productId);
        Task<int> GetTotalQuantityForProduct(int productId);
        Task<List<InfoStockWarningDTO>> GetWarningStocks();
        Task<List<InfoStockTransactionDTO>> GetStockTransactionsByProductName(string productName, DateTime? from = null, DateTime? to = null);
        Task<List<InfoStockModificationDTO>> GetStockModificationsByProductName(string productName, DateTime? from = null, DateTime? to = null);
        Task<List<InfoStockSummaryDTO>> GetStockSummaries(string productName, DateTime? from = null, DateTime? to = null);
        Task<int> CreateStock(AddStockDTO stock);
        Task<bool> UpdateStockData(EditStockDTO stockDto);
        Task MarkExpiredAsInactiveAndMarkWarning(int productId);
        Task ReduceStockData(EditReduceStockDTO reduceStockDTO);
        Task<bool> RefundStock(int orderId);
    }
}