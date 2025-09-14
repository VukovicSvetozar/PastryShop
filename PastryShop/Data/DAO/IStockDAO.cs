using PastryShop.Data.DTO;
using PastryShop.Model;

namespace PastryShop.Data.DAO
{
    public interface IStockDAO
    {
        Task<List<Stock>> GetAllStocksByProductId(int productId);
        Task<int> GetTotalQuantityForProduct(int productId);
        Task<Stock> GetStockById(int id);
        Task<List<InfoStockWarningDTO>> GetWarningStocks();
        Task<List<StockTransaction>> GetStockTransactionsByOrderId(int orderId);
        Task<List<StockTransaction>> GetStockTransactionsByProductName(string productName, DateTime? from, DateTime? to);
        Task<List<StockModification>> GetStockModificationsByProductName(string productName, DateTime? from, DateTime? to);
        Task<List<InfoStockSummaryDTO>> GetStockSummaries(string productName, DateTime? from, DateTime? to);
        Task<int> AddStock(Stock stock);
        Task<int> AddStockTransaction(StockTransaction stockTransaction);
        Task<int> AddStockModification(StockModification stockModification);
        Task<bool> UpdateStock(Stock stock);
        Task<bool> UpdateStockTransaction(StockTransaction stockTransaction);
    }
}