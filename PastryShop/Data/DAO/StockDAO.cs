using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;
using PastryShop.Data.DTO;
using PastryShop.Enum;
using PastryShop.Model;
using PastryShop.Utility;

namespace PastryShop.Data.DAO
{
    public class StockDao(string connectionString) : IStockDAO
    {
        private readonly string _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));

        public async Task<List<Stock>> GetAllStocksByProductId(int productId)
        {
            const string query = @"
                SELECT Id, ProductId, Quantity, ExpirationDate, ExpirationWarningDays, IsActive, IsWarning, AddedDate
                FROM Stocks
                WHERE ProductId = @ProductId";

            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@ProductId", productId }
                };

                var reader = (DbDataReader)await ExecuteCommandAsync(query, true, parameters);
                try
                {
                    var stockList = new List<Stock>();

                    while (await reader.ReadAsync())
                    {
                        stockList.Add(MapStock(reader));
                    }

                    return stockList;
                }
                finally
                {
                    await reader.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška prilikom dohvatanja zaliha za proizvod ID: {productId}", ex);
                return [];
            }
        }

        public async Task<int> GetTotalQuantityForProduct(int productId)
        {
            const string query = @"
                SELECT COALESCE(SUM(Quantity), 0) AS TotalQuantity
                FROM Stocks
                WHERE ProductId = @ProductId AND IsActive = 1";

            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@ProductId", productId }
                };

                var reader = (DbDataReader)await ExecuteCommandAsync(query, true, parameters);
                try
                {
                    if (await reader.ReadAsync())
                    {
                        return Convert.ToInt32(reader["TotalQuantity"]);
                    }
                    return 0;
                }
                finally
                {
                    await reader.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška prilikom dohvatanja ukupne količine za proizvod sa ID: {productId}", ex);
                return 0;
            }
        }

        public async Task<Stock> GetStockById(int id)
        {
            const string query = @"
                SELECT Id, ProductId, Quantity, ExpirationDate, ExpirationWarningDays, IsActive, IsWarning, AddedDate
                FROM Stocks
                WHERE Id = @Id";

            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@Id", id }
                };

                var reader = (DbDataReader)await ExecuteCommandAsync(query, true, parameters);
                try
                {
                    if (await reader.ReadAsync())
                    {
                        return MapStock(reader);
                    }
                    else
                    {
                        Logger.LogError($"Stock sa ID: {id} nije pronađen.", new KeyNotFoundException());
                        return new Stock();
                    }
                }
                finally
                {
                    await reader.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška prilikom dohvatanja zaliha za ID: {id}", ex);
                return new Stock();
            }
        }

        public async Task<List<InfoStockWarningDTO>> GetWarningStocks()
        {
            const string sql = @"
                SELECT
                    s.Id AS StockId,
                    s.ProductId,
                    p.Name AS ProductName,
                    s.Quantity,
                    s.ExpirationDate,
                    s.ExpirationWarningDays
                FROM Stocks AS s
                JOIN Products AS p
                    ON p.Id = s.ProductId
                WHERE s.IsActive  = 1
                    AND s.IsWarning = 1
                ";

            var list = new List<InfoStockWarningDTO>();
            var reader = (DbDataReader)await ExecuteCommandAsync(sql, true, null);
            try
            {
                while (await reader.ReadAsync())
                {
                    list.Add(new InfoStockWarningDTO
                    {
                        StockId = reader.GetInt32("StockId"),
                        ProductId = reader.GetInt32("ProductId"),
                        ProductName = reader.GetString("ProductName"),
                        Quantity = reader.GetInt32("Quantity"),
                        ExpirationDate = reader.IsDBNull("ExpirationDate")
                            ? (DateTime?)null : reader.GetDateTime("ExpirationDate"),
                        ExpirationWarningDays = reader.IsDBNull("ExpirationWarningDays")
                            ? (int?)null : reader.GetInt32("ExpirationWarningDays")
                    });
                }
            }
            finally
            {
                await reader.CloseAsync();
            }

            return list;
        }

        public async Task<List<StockTransaction>> GetStockTransactionsByOrderId(int orderId)
        {
            const string query = @"
                SELECT Id, StockId, ProductId, OrderId, UserId, QuantityChanged, TransactionDate, TransactionType
                FROM StockTransactions
                WHERE OrderId = @OrderId AND OrderId IS NOT NULL";

            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@OrderId", orderId }
                };

                var reader = (DbDataReader)await ExecuteCommandAsync(query, true, parameters);
                try
                {
                    var stockTransactions = await MapReaderToStockTransactions(reader);
                    return stockTransactions;
                }
                finally
                {
                    await reader.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška prilikom dohvatanja transakcija zaliha za OrderId: {orderId}.", ex);
                return [];
            }
        }

        public async Task<List<StockTransaction>> GetStockTransactionsByProductName(string productName, DateTime? from, DateTime? to)
        {
            const string query = @"
                SELECT t.Id, t.StockId, t.ProductId, t.OrderId, t.UserId, t.QuantityChanged, t.TransactionDate, t.TransactionType
                FROM StockTransactions AS t
                JOIN Products AS p ON p.Id = t.ProductId
                WHERE p.Name = @productName AND (@from IS NULL OR t.TransactionDate >= @from) AND (@to IS NULL OR t.TransactionDate < @to)
                ORDER BY t.TransactionDate";

            try
            {
                var toAdjusted = to?.Date.AddDays(1);
                var parameters = new Dictionary<string, object>
                    {
                        { "@productName", productName },
                        { "@from", from.HasValue ? (object)from.Value : DBNull.Value },
                        { "@to", toAdjusted.HasValue ? (object)toAdjusted.Value : DBNull.Value }
                    };

                var reader = (DbDataReader)await ExecuteCommandAsync(query, true, parameters);
                try
                {
                    var stockTransactions = await MapReaderToStockTransactions(reader);
                    return stockTransactions;
                }
                finally
                {
                    await reader.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška prilikom dohvatanja transakcija zaliha za naziv proizvoda: {productName}.", ex);
                return [];
            }
        }

        public async Task<List<StockModification>> GetStockModificationsByProductName(string productName, DateTime? from, DateTime? to)
        {
            const string query = @"
                SELECT m.Id, m.StockId, m.ProductId, m.UserId, m.OldValue, m.NewValue, m.ModificationDate, m.ModificationType
                    FROM stockModifications AS m
                    JOIN Stocks AS s ON s.Id = m.StockId
                    JOIN Products AS p ON p.Id = s.ProductId
                    WHERE p.Name = @productName AND (@from IS NULL OR m.ModificationDate >= @from) AND (@to IS NULL OR m.ModificationDate < @to)
                    ORDER BY m.ModificationDate";

            try
            {
                var toAdjusted = to?.Date.AddDays(1);
                var parameters = new Dictionary<string, object>
                    {
                        { "@productName", productName },
                        { "@from", from.HasValue ? (object)from.Value : DBNull.Value },
                        { "@to", toAdjusted.HasValue ? (object)toAdjusted.Value : DBNull.Value }
                    };

                var reader = (DbDataReader)await ExecuteCommandAsync(query, true, parameters);
                try
                {
                    var stockModifications = await MapReaderToStockModifications(reader);
                    return stockModifications;
                }
                finally
                {
                    await reader.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška prilikom dohvatanja modifikacija zaliha za naziv proizvoda: {productName}.", ex);
                return [];
            }
        }

        public async Task<List<InfoStockSummaryDTO>> GetStockSummaries(string productName, DateTime? from, DateTime? to)
        {
            const string sql = @"
            WITH txn AS (
                SELECT 
                    t.ProductId,
                    SUM(t.QuantityChanged) AS TotalQuantity,
                    SUM(CASE WHEN t.TransactionType = 'Addition' THEN t.QuantityChanged ELSE 0 END) AS TotalAdded,
                    SUM(CASE WHEN t.TransactionType = 'Sale' THEN -t.QuantityChanged ELSE 0 END) AS TotalSold
                FROM StockTransactions AS t 
                    JOIN Products AS p ON p.Id = t.ProductId
                WHERE p.Name = @productName AND (@from IS NULL OR t.TransactionDate >= @from) AND (@to IS NULL OR t.TransactionDate < @to)
                GROUP BY t.ProductId
            ),
            modifications AS (
                SELECT
                    m.ProductId,
                    COUNT(*) AS TotalModifications
                FROM stockModifications AS m
                    JOIN Stocks AS s ON s.Id = m.StockId
                    JOIN Products AS p ON p.Id = s.ProductId
                WHERE p.Name = @productName AND (@from IS NULL OR m.ModificationDate >= @from) AND (@to IS NULL OR m.ModificationDate < @to)
                GROUP BY m.ProductId
            )
            SELECT
                t.ProductId,
                t.TotalQuantity,
                t.TotalAdded,
                t.TotalSold,
                COALESCE(m.TotalModifications, 0) AS TotalModifications
                FROM txn AS t LEFT JOIN modifications AS m ON m.ProductId = t.ProductId
                ORDER BY t.ProductId; ";

            var toAdjusted = to?.Date.AddDays(1);
            var parameters = new Dictionary<string, object>
                {
                    { "@productName", productName },
                    { "@from", from.HasValue ? (object)from.Value : DBNull.Value },
                    { "@to", toAdjusted.HasValue ? (object)toAdjusted.Value : DBNull.Value }
                };

            var list = new List<InfoStockSummaryDTO>();
            var reader = (DbDataReader)await ExecuteCommandAsync(sql, true, parameters);
            try
            {
                while (await reader.ReadAsync())
                {
                    list.Add(new InfoStockSummaryDTO
                    {
                        ProductId = reader.GetInt32("ProductId"),
                        TotalQuantity = reader.GetInt32("TotalQuantity"),
                        TotalAdded = reader.GetInt32("TotalAdded"),
                        TotalSold = reader.GetInt32("TotalSold"),
                        TotalModifications = reader.GetInt32("TotalModifications")
                    });
                }
            }
            finally
            {
                await reader.CloseAsync();
            }

            return list;
        }

        public async Task<int> AddStock(Stock stock)
        {
            const string stockQuery = @"
                INSERT INTO Stocks 
                    (ProductId, Quantity, ExpirationDate, ExpirationWarningDays, IsActive, IsWarning, AddedDate) 
                VALUES 
                    (@ProductId, @Quantity, @ExpirationDate, @ExpirationWarningDays, @IsActive, @IsWarning, @AddedDate);
                SELECT LAST_INSERT_ID();";

            var parameters = new Dictionary<string, object>
                {
                    { "@ProductId", stock.ProductId },
                    { "@Quantity", stock.Quantity },
                    { "@ExpirationDate", stock.ExpirationDate ?? (object)DBNull.Value },
                    { "@ExpirationWarningDays", stock.ExpirationWarningDays ?? (object)DBNull.Value },
                    { "@IsActive" , stock.IsActive },
                    { "@IsWarning" , stock.IsWarning },
                    { "@AddedDate", stock.AddedDate }
                };
            try
            {
                DbDataReader? reader = null;

                try
                {
                    reader = (DbDataReader)await ExecuteCommandAsync(stockQuery, true, parameters);

                    int stockId = 0;

                    if (await reader.ReadAsync())
                    {
                        stockId = Convert.ToInt32(reader[0]);
                    }
                    return stockId;
                }
                finally
                {
                    if (reader != null && !reader.IsClosed)
                    {
                        await reader.CloseAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška prilikom dodavanja zaliha za proizvod ID: {stock.ProductId}", ex);
                return 0;
            }
        }

        public async Task<int> AddStockTransaction(StockTransaction stockTransaction)
        {
            const string stockTransactionQuery = @"
                INSERT INTO StockTransactions
                    (StockId, ProductId, OrderId, UserId, QuantityChanged, TransactionDate, TransactionType) 
                VALUES 
                    (@StockId, @ProductId, @OrderId, @UserId, @QuantityChanged, @TransactionDate, @TransactionType);
                SELECT LAST_INSERT_ID();";

            var parameters = new Dictionary<string, object>
                {
                    { "@StockId", stockTransaction.StockId },
                    { "@ProductId", stockTransaction.ProductId },
                    { "@OrderId", stockTransaction.OrderId ?? (object)DBNull.Value },
                    { "@UserId", stockTransaction.UserId },
                    { "@QuantityChanged", stockTransaction.QuantityChanged },
                    { "@TransactionDate", stockTransaction.TransactionDate },
                    { "@TransactionType", stockTransaction.TransactionType.ToString() }
                };

            try
            {
                DbDataReader? reader = null;

                try
                {
                    reader = (DbDataReader)await ExecuteCommandAsync(stockTransactionQuery, true, parameters);

                    int stockTransactionId = 0;

                    if (await reader.ReadAsync())
                    {
                        stockTransactionId = Convert.ToInt32(reader[0]);
                    }
                    return stockTransactionId;
                }
                finally
                {
                    if (reader != null && !reader.IsClosed)
                    {
                        await reader.CloseAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška prilikom kreiranja transakcija zaliha za proizvod ID: {stockTransaction.ProductId}", ex);
                return 0;
            }
        }

        public async Task<int> AddStockModification(StockModification stockModification)
        {
            const string stockModificationQuery = @"
                    INSERT INTO StockModifications
                        (StockId, ProductId, UserId, OldValue, NewValue, ModificationDate, ModificationType) 
                    VALUES 
                        (@StockId, @ProductId, @UserId, @OldValue, @NewValue, @ModificationDate, @ModificationType);
                    SELECT LAST_INSERT_ID();";

            var parameters = new Dictionary<string, object>
                {
                    { "@StockId", stockModification.StockId },
                    { "@ProductId", stockModification.ProductId },
                    { "@UserId", stockModification.UserId },
                    { "@OldValue", stockModification.OldValue },
                    { "@NewValue", stockModification.NewValue },
                    { "@ModificationDate", stockModification.ModificationDate },
                    { "@ModificationType", stockModification.ModificationType.ToString() }
                };

            try
            {
                DbDataReader? reader = null;

                try
                {
                    reader = (DbDataReader)await ExecuteCommandAsync(stockModificationQuery, true, parameters);
                    int stockModificationId = 0;

                    if (await reader.ReadAsync())
                    {

                        stockModificationId = Convert.ToInt32(reader[0]);

                    }
                    return stockModificationId;
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Greška", ex);
                    return 0;
                }
                finally
                {
                    if (reader != null && !reader.IsClosed)
                    {
                        await reader.CloseAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška prilikom kreiranja modifikacije zaliha za proizvod ID: {stockModification.ProductId}", ex);
                return 0;
            }
        }

        public async Task<bool> UpdateStock(Stock stock)
        {
            const string query = @"
                UPDATE Stocks
                SET ProductId = @ProductId,
                    Quantity = @Quantity,
                    ExpirationDate = @ExpirationDate,
                    ExpirationWarningDays = @ExpirationWarningDays,
                    IsActive = @IsActive,
                    IsWarning = @IsWarning,
                    AddedDate = @AddedDate
                WHERE Id = @Id";
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@Id", stock.Id },
                    { "@ProductId", stock.ProductId },
                    { "@Quantity", stock.Quantity },
                    { "@ExpirationDate", stock.ExpirationDate ?? (object)DBNull.Value },
                    { "@ExpirationWarningDays", stock.ExpirationWarningDays ?? (object)DBNull.Value },
                    { "@IsActive", stock.IsActive },
                    { "@IsWarning", stock.IsWarning },
                    { "@AddedDate", stock.AddedDate }
                };

                var rowsAffected = (int)await ExecuteCommandAsync(query, false, parameters);

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška prilikom ažuriranja zaliha za ID: {stock.Id}", ex);
                return false;
            }
        }

        public async Task<bool> UpdateStockTransaction(StockTransaction stockTransaction)
        {
            const string query = @"
                UPDATE StockTransactions
                SET StockId = @StockId,
                    ProductId = @ProductId,
                    OrderId = @OrderId,
                    UserId = @UserId,
                    QuantityChanged = @QuantityChanged,
                    TransactionDate = @TransactionDate,
                    TransactionType = @TransactionType
                WHERE Id = @Id";
            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@Id", stockTransaction.Id },
                    { "@StockId", stockTransaction.StockId },
                    { "@ProductId", stockTransaction.ProductId },
                    { "@OrderId", stockTransaction.OrderId ?? (object)DBNull.Value },
                    { "@UserId", stockTransaction.UserId },
                    { "@QuantityChanged", stockTransaction.QuantityChanged },
                    { "@TransactionDate", stockTransaction.TransactionDate },
                    { "@TransactionType", stockTransaction.TransactionType.ToString() }
                };

                var rowsAffected = (int)await ExecuteCommandAsync(query, false, parameters);

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška prilikom ažuriranja transakcija zaliha za ID: {stockTransaction.Id}", ex);
                return false;
            }
        }

        private async Task<object> ExecuteCommandAsync(string query, bool isReader, Dictionary<string, object>? parameters = null)
        {
            try
            {
                var connection = new MySqlConnection(_connectionString);
                await connection.OpenAsync();

                var command = new MySqlCommand(query, connection);

                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        command.Parameters.AddWithValue(param.Key, param.Value);
                    }
                }

                if (isReader)
                {
                    return await command.ExecuteReaderAsync(CommandBehavior.CloseConnection);
                }
                else
                {
                    using (connection)
                    using (command)
                    {
                        var result = await command.ExecuteNonQueryAsync();
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Greška prilikom izvršavanja SQL komande.", ex);
                if (isReader)
                    return new List<object>();
                else
                    return 0;
            }
        }

        private static Stock MapStock(DbDataReader reader)
        {
            return new Stock
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                ExpirationDate = reader.IsDBNull(reader.GetOrdinal("ExpirationDate"))
                    ? null : reader.GetDateTime(reader.GetOrdinal("ExpirationDate")),
                ExpirationWarningDays = reader.IsDBNull(reader.GetOrdinal("ExpirationWarningDays"))
                    ? null : reader.GetInt32(reader.GetOrdinal("ExpirationWarningDays")),
                IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                IsWarning = reader.GetBoolean(reader.GetOrdinal("IsWarning")),
                AddedDate = reader.GetDateTime(reader.GetOrdinal("AddedDate"))
            };
        }

        private static async Task<List<StockTransaction>> MapReaderToStockTransactions(DbDataReader reader)
        {
            if (reader.IsClosed)
                throw new InvalidOperationException("Reader je zatvoren prije mapiranja podataka.");

            var list = new List<StockTransaction>();
            while (await reader.ReadAsync())
            {
                if (reader.IsClosed)
                    throw new InvalidOperationException("Reader je zatvoren tokom mapiranja podataka.");
                list.Add(MapStockTransaction(reader));
            }

            return list;
        }

        private static async Task<List<StockModification>> MapReaderToStockModifications(DbDataReader reader)
        {
            if (reader.IsClosed)
                throw new InvalidOperationException("Reader je zatvoren prije mapiranja podataka.");

            var list = new List<StockModification>();
            while (await reader.ReadAsync())
            {
                if (reader.IsClosed)
                    throw new InvalidOperationException("Reader je zatvoren tokom mapiranja podataka.");
                list.Add(MapStockModification(reader));
            }
            return list;
        }

        private static StockTransaction MapStockTransaction(DbDataReader reader)
        {
            return new StockTransaction
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                StockId = reader.GetInt32(reader.GetOrdinal("StockId")),
                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                OrderId = reader.IsDBNull(reader.GetOrdinal("OrderId"))
                    ? (int?)null : reader.GetInt32(reader.GetOrdinal("OrderId")),
                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                QuantityChanged = reader.GetInt32(reader.GetOrdinal("QuantityChanged")),
                TransactionDate = reader.GetDateTime(reader.GetOrdinal("TransactionDate")),
                TransactionType = System.Enum.Parse<TransactionType>(reader.GetString("TransactionType"))
            };
        }

        private static StockModification MapStockModification(DbDataReader reader)
        {
            return new StockModification
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                StockId = reader.GetInt32(reader.GetOrdinal("StockId")),
                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                OldValue = reader.GetString(reader.GetOrdinal("OldValue")),
                NewValue = reader.GetString(reader.GetOrdinal("NewValue")),
                ModificationDate = reader.GetDateTime(reader.GetOrdinal("ModificationDate")),
                ModificationType = System.Enum.Parse<ModificationType>(reader.GetString("ModificationType"))
            };
        }

    }
}