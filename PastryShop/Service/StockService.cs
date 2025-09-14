using PastryShop.Model;
using PastryShop.Data.DAO;
using PastryShop.Data.DTO;
using PastryShop.Enum;
using PastryShop.Utility;

namespace PastryShop.Service
{
    public class StockService(IStockDAO stockDao) : IStockService
    {
        private readonly IStockDAO _stockDao = stockDao;

        public async Task<List<InfoStockDTO>> GetAllStocksByProductId(int productId)
        {
            var stocks = await _stockDao.GetAllStocksByProductId(productId);

            var stockDtos = stocks.Select(stock => new InfoStockDTO
            {
                Id = stock.Id,
                ProductId = stock.ProductId,
                Quantity = stock.Quantity,
                ExpirationDate = stock.ExpirationDate,
                ExpirationWarningDays = stock.ExpirationWarningDays,
                IsActive = stock.IsActive,
                IsWarning = stock.IsWarning,
                AddedDate = stock.AddedDate
            }).ToList();

            return stockDtos;
        }

        public async Task<int> GetTotalQuantityForProduct(int productId)
        {
            return await _stockDao.GetTotalQuantityForProduct(productId);
        }

        public async Task<List<InfoStockWarningDTO>> GetWarningStocks()
        {
            var data = await _stockDao.GetWarningStocks();
            return data;
        }

        public async Task<List<InfoStockTransactionDTO>> GetStockTransactionsByProductName(string productName, DateTime? from = null, DateTime? to = null)
        {
            if (string.IsNullOrWhiteSpace(productName))
            {
                Logger.LogError("Unesite ime proizvoda.", new ArgumentException("Unesite ime proizvoda.", nameof(productName)));
                return [];
            }

            var entities = await _stockDao.GetStockTransactionsByProductName(productName, from, to);
            var dtos = entities.Select(stockTransaction => new InfoStockTransactionDTO
            {
                Id = stockTransaction.Id,
                StockId = stockTransaction.StockId,
                ProductId = stockTransaction.ProductId,
                OrderId = stockTransaction.OrderId,
                UserId = stockTransaction.UserId,
                QuantityChanged = stockTransaction.QuantityChanged,
                TransactionDate = stockTransaction.TransactionDate,
                TransactionType = stockTransaction.TransactionType
            }).ToList();

            return dtos;
        }

        public async Task<List<InfoStockModificationDTO>> GetStockModificationsByProductName(string productName, DateTime? from = null, DateTime? to = null)
        {
            if (string.IsNullOrWhiteSpace(productName))
            {
                Logger.LogError("Unesite ime proizvoda.", new ArgumentException("Unesite ime proizvoda.", nameof(productName)));
                return [];
            }

            var entities = await _stockDao.GetStockModificationsByProductName(productName, from, to);
            var dtos = entities.Select(mod => new InfoStockModificationDTO
            {
                Id = mod.Id,
                StockId = mod.StockId,
                ProductId = mod.ProductId,
                UserId = mod.UserId,
                OldValue = mod.OldValue,
                NewValue = mod.NewValue,
                ModificationDate = mod.ModificationDate,
                ModificationType = mod.ModificationType
            }).ToList();

            return dtos;
        }

        public async Task<List<InfoStockSummaryDTO>> GetStockSummaries(string productName, DateTime? from = null, DateTime? to = null)
        {
            if (string.IsNullOrWhiteSpace(productName))
            {
                Logger.LogError("Unesite naziv proizvoda.", new ArgumentException("Unesite ime proizvoda.", nameof(productName)));
                return [];
            }

            return await _stockDao.GetStockSummaries(productName, from, to);
        }

        public async Task<int> CreateStock(AddStockDTO stockDto)
        {
            if (stockDto == null)
            {
                Logger.LogError("Stock DTO je null.", new ArgumentNullException(nameof(stockDto)));
                return 0;
            }

            var stockEntity = new Stock
            {
                ProductId = stockDto.ProductId,
                Quantity = stockDto.Quantity,
                ExpirationDate = stockDto.ExpirationDate,
                ExpirationWarningDays = stockDto.ExpirationWarningDays,
                IsActive = true,
                AddedDate = DateTime.Now
            };

            if (stockEntity.ExpirationDate.HasValue && stockEntity.ExpirationWarningDays.HasValue)
            {
                var daysLeft = (stockEntity.ExpirationDate.Value.Date - DateTime.Today).Days;
                stockEntity.IsWarning = daysLeft >= 0 && daysLeft <= stockEntity.ExpirationWarningDays.Value;
            }
            else
            {
                stockEntity.IsWarning = false;
            }

            var newStockId = await _stockDao.AddStock(stockEntity);

            var stockTransaction = new StockTransaction
            {
                StockId = newStockId,
                ProductId = stockDto.ProductId,
                OrderId = null,
                UserId = stockDto.UserId,
                QuantityChanged = stockDto.Quantity,
                TransactionDate = DateTime.Now,
                TransactionType = TransactionType.Addition
            };

            await _stockDao.AddStockTransaction(stockTransaction);

            return newStockId;
        }

        public async Task<bool> UpdateStockData(EditStockDTO stockDto)
        {
            var oldStock = await _stockDao.GetStockById(stockDto.Id);

            if (oldStock == null)
            {
                Logger.LogError($"Stock sa ID {stockDto.Id} nije pronađen.", new ApplicationException());
                return false;
            }

            var newStock = new Stock
            {
                Id = stockDto.Id,
                ProductId = oldStock.ProductId,
                Quantity = stockDto.Quantity ?? oldStock.Quantity,
                ExpirationDate = stockDto.ExpirationDate ?? oldStock.ExpirationDate,
                ExpirationWarningDays = stockDto.ExpirationWarningDays ?? oldStock.ExpirationWarningDays,
                IsActive = stockDto.IsActive ?? oldStock.IsActive,
                IsWarning = stockDto.IsWarning ?? oldStock.IsWarning,
                AddedDate = oldStock.AddedDate
            };

            if (stockDto.Quantity.HasValue && stockDto.Quantity.Value != oldStock.Quantity)
            {
                int quantityDifference = stockDto.Quantity.Value - oldStock.Quantity;
                var stockTransaction = new StockTransaction
                {
                    StockId = newStock.Id,
                    ProductId = newStock.ProductId,
                    OrderId = stockDto.OrderId,
                    UserId = stockDto.UserId,
                    QuantityChanged = quantityDifference,
                    TransactionDate = DateTime.Now,
                    TransactionType = stockDto.TransactionType
                };

                await _stockDao.AddStockTransaction(stockTransaction);
            }


            if (stockDto.ExpirationDate.HasValue &&
                (!oldStock.ExpirationDate.HasValue || stockDto.ExpirationDate.Value != oldStock.ExpirationDate.Value))
            {
                var modification = new StockModification
                {
                    StockId = oldStock.Id,
                    ProductId = oldStock.ProductId,
                    UserId = stockDto.UserId,
                    OldValue = oldStock.ExpirationDate?.ToString("yyyy-MM-dd") ?? "null",
                    NewValue = stockDto.ExpirationDate.Value.ToString("yyyy-MM-dd"),
                    ModificationDate = DateTime.Now,
                    ModificationType = ModificationType.ExpirationDateChange
                };

                await _stockDao.AddStockModification(modification);
            }

            if (stockDto.ExpirationWarningDays.HasValue &&
                (!oldStock.ExpirationWarningDays.HasValue || stockDto.ExpirationWarningDays.Value != oldStock.ExpirationWarningDays.Value))
            {
                var modification = new StockModification
                {
                    StockId = oldStock.Id,
                    ProductId = oldStock.ProductId,
                    UserId = stockDto.UserId,
                    OldValue = oldStock.ExpirationWarningDays?.ToString() ?? "null",
                    NewValue = stockDto.ExpirationWarningDays.Value.ToString(),
                    ModificationDate = DateTime.Now,
                    ModificationType = ModificationType.WarningDaysChange
                };

                await _stockDao.AddStockModification(modification);
            }

            if (stockDto.IsActive.HasValue && stockDto.IsActive.Value != oldStock.IsActive)
            {
                var modification = new StockModification
                {
                    StockId = oldStock.Id,
                    ProductId = oldStock.ProductId,
                    UserId = UserSession.GetCurrentUser().Id,
                    OldValue = oldStock.IsActive.ToString(),
                    NewValue = stockDto.IsActive.Value.ToString(),
                    ModificationDate = DateTime.Now,
                    ModificationType = ModificationType.StatusChange
                };
                await _stockDao.AddStockModification(modification);
            }

            return await _stockDao.UpdateStock(newStock);

        }

        public async Task MarkExpiredAsInactiveAndMarkWarning(int productId)
        {
            var stocks = await GetAllStocksByProductId(productId);

            foreach (var s in stocks)
            {
                if (s.IsActive && s.ExpirationDate.HasValue && s.ExpirationDate.Value.Date < DateTime.Today)
                {
                    s.IsActive = false;
                    s.IsWarning = false;
                }
            }
            foreach (var s in stocks)
            {
                if (s.IsActive && s.ExpirationDate.HasValue && s.ExpirationWarningDays.HasValue)
                {
                    var daysLeft = (s.ExpirationDate.Value.Date - DateTime.Today).Days;
                    s.IsWarning = daysLeft >= 0 && daysLeft <= s.ExpirationWarningDays.Value;
                }
            }

            foreach (var s in stocks)
            {
                var dto = new EditStockDTO
                {
                    Id = s.Id,
                    IsActive = s.IsActive,
                    IsWarning = s.IsWarning
                };
                await UpdateStockData(dto);
            }

        }

        public async Task ReduceStockData(EditReduceStockDTO reduceStockDTO)
        {
            int remaining = reduceStockDTO.QuantityToReduce;

            List<Stock> _stocks = await _stockDao.GetAllStocksByProductId(reduceStockDTO.ProductId);
            var availableStocks = _stocks
                .Where(s => s.ProductId == reduceStockDTO.ProductId && s.IsActive && s.Quantity > 0)
                .OrderBy(s => s.AddedDate)
                .ToList();

            foreach (var stock in availableStocks)
            {
                if (remaining <= 0)
                    break;

                if (stock.Quantity >= remaining)
                {
                    stock.Quantity -= remaining;
                    if (stock.Quantity == 0)
                        stock.IsActive = false;

                    var editStockDTO = new EditStockDTO
                    {
                        Id = stock.Id,
                        UserId = reduceStockDTO.UserId,
                        OrderId = reduceStockDTO.OrderId,
                        Quantity = stock.Quantity,
                        IsActive = stock.IsActive,
                        IsWarning = stock.IsWarning,
                        TransactionType = TransactionType.Sale
                    };
                    await UpdateStockData(editStockDTO);

                    remaining = 0;
                }
                else
                {
                    int deducted = stock.Quantity;
                    remaining -= deducted;
                    stock.Quantity = 0;

                    var editStockDTO = new EditStockDTO
                    {
                        Id = stock.Id,
                        UserId = reduceStockDTO.UserId,
                        OrderId = reduceStockDTO.OrderId,
                        Quantity = 0,
                        IsActive = false,
                        IsWarning = false,
                        TransactionType = TransactionType.Sale
                    };
                    await UpdateStockData(editStockDTO);
                }
            }

        }

        public async Task<bool> RefundStock(int orderId)
        {
            var transactions = await _stockDao.GetStockTransactionsByOrderId(orderId);

            if (transactions == null || transactions.Count == 0)
            {
                return false;
            }

            foreach (var transaction in transactions)
            {
                if (transaction.TransactionType != TransactionType.Return)
                {
                    transaction.TransactionType = TransactionType.Return;
                    transaction.QuantityChanged = Math.Abs(transaction.QuantityChanged);
                    transaction.TransactionDate = DateTime.Now;
                    await _stockDao.UpdateStockTransaction(transaction);
                }

                var stock = await _stockDao.GetStockById(transaction.StockId);
                if (stock == null)
                {
                    continue;
                }
                stock.Quantity += transaction.QuantityChanged;

                if (!stock.IsActive)
                {
                    stock.IsActive = true;

                    var modification = new StockModification
                    {
                        StockId = transaction.StockId,
                        ProductId = transaction.ProductId,
                        UserId = transaction.UserId,
                        OldValue = "False",
                        NewValue = "True",
                        ModificationDate = DateTime.Now,
                        ModificationType = ModificationType.StatusChange
                    };
                    await _stockDao.AddStockModification(modification);
                }

                await _stockDao.UpdateStock(stock);

            }

            return true;
        }

    }
}