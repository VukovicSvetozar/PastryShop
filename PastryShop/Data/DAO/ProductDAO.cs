using System.Text;
using System.Data;
using System.Data.Common;
using MySql.Data.MySqlClient;
using PastryShop.Model;
using PastryShop.Enum;
using PastryShop.Data.DTO;
using PastryShop.Utility;

namespace PastryShop.Data.DAO
{
    public class ProductDao(string connectionString) : IProductDAO
    {
        private readonly string _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));

        public async Task<Product?> GetProductById(int productId)
        {
            const string query = @"
                SELECT p.*, f.*, d.*, a.*
                FROM Products p
                LEFT JOIN FoodProducts f ON p.Id = f.ProductId
                LEFT JOIN DrinkProducts d ON p.Id = d.ProductId
                LEFT JOIN AccessoryProducts a ON p.Id = a.ProductId
                WHERE p.Id = @ProductId";

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
                        return MapReaderToProduct(reader);
                    }
                    return null;
                }
                finally
                {
                    await reader.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška prilikom dohvatanja proizvoda za ID: {productId}", ex);
                return null;
            }
        }

        public async Task<List<Product>> GetAllProducts()
        {
            const string query = @"
                SELECT p.*, f.*, d.*, a.*
                FROM Products p
                LEFT JOIN FoodProducts f ON p.Id = f.ProductId
                LEFT JOIN DrinkProducts d ON p.Id = d.ProductId
                LEFT JOIN AccessoryProducts a ON p.Id = a.ProductId";

            try
            {
                var reader = (DbDataReader)await ExecuteCommandAsync(query, true);
                try
                {
                    var products = await MapReaderToProducts(reader);
                    return products;
                }
                finally
                {
                    await reader.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Greška prilikom dohvatanja svih proizvoda iz baze.", ex);
                return [];
            }
        }

        public async Task<List<string>> GetAllProductNames()
        {
            const string query = @"
                SELECT Name
                FROM Products
                ORDER BY Name";

            try
            {
                var names = new List<string>();
                var reader = (DbDataReader)await ExecuteCommandAsync(query, true);
                try
                {
                    while (await reader.ReadAsync())
                    {
                        names.Add(reader.GetString(0));
                    }
                    return names;
                }
                finally
                {
                    await reader.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Greška prilikom dohvatanja imena proizvoda iz baze.", ex);
                return [];
            }
        }

        public async Task<List<Order>> GetRecentOrdersForUser(int userId)
        {
            const string query = @"
                SELECT Id, UserId, OrderDate, TotalPrice, Status
                FROM Orders
                WHERE OrderDate >= TIMESTAMPADD(hour, -8, NOW())
                AND UserId = @UserId";

            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@UserId", userId }
                };

                var reader = (DbDataReader)await ExecuteCommandAsync(query, true, parameters);
                try
                {
                    var orders = await MapReaderToOrders(reader);
                    return orders;
                }
                finally
                {
                    await reader.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Greška prilikom dohvatanja nedavnih narudžbi za korisnika.", ex);
                return [];
            }
        }

        public async Task<Order?> GetOrderById(int id)
        {
            const string query = @"
                SELECT Id, UserId, OrderDate, TotalPrice, Status
                FROM Orders
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
                        return MapOrder(reader);
                    }
                    else
                    {
                        Logger.LogError($"Order sa ID: {id} nije pronađen.", new KeyNotFoundException());
                        return null;
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
                return null;
            }
        }

        public async Task<List<OrderItem>> GetOrderItemsByOrderId(int orderId)
        {
            const string query = @"
                SELECT Id, OrderId, ProductId, Quantity, UnitPrice
                FROM OrderItems
                WHERE OrderId = @OrderId AND OrderId IS NOT NULL"
            ;

            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@OrderId", orderId }
                };

                var reader = (DbDataReader)await ExecuteCommandAsync(query, true, parameters);
                try
                {
                    var orderItems = await MapReaderToOrderItems(reader);
                    return orderItems;
                }
                finally
                {
                    await reader.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška prilikom dohvatanja stavki narudžbe za OrderId: {orderId}.", ex);
                return [];
            }
        }

        public async Task<Payment?> GetPaymentByOrderId(int orderId)
        {
            const string query = @"
                SELECT Id, UserId, OrderId, PaymentMethod, PaymentStatus, AmountPaid, CardNumber, PaymentDate
                FROM Payments
                WHERE OrderId = @OrderId AND OrderId IS NOT NULL"
            ;

            try
            {
                var parameters = new Dictionary<string, object>
                {
                     { "@OrderId", orderId }
                };

                var reader = (DbDataReader)await ExecuteCommandAsync(query, true, parameters);
                try
                {
                    if (await reader.ReadAsync())
                    {
                        return MapPayment(reader);
                    }
                    else
                    {
                        Logger.LogError($"Payment sa OrderId: {orderId} nije pronađen.", new KeyNotFoundException());
                        return null;
                    }
                }
                finally
                {
                    await reader.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška prilikom dohvatanja plaćanja za OrderId: {orderId}", ex);
                return null;
            }
        }

        public async Task<List<InfoProductTopSalesStatsDTO>> GetBestSellersProducts(int limit)
        {
            string query = $@"
                SELECT 
                    p.Id             AS Id,
                    p.ProductType    AS ProductType,
                    p.Name           AS Name,
                    p.ImagePath      AS ImagePath,
                    SUM(oi.Quantity) AS TotalSold
                FROM OrderItems oi
                INNER JOIN Orders o    ON oi.OrderId   = o.Id
                INNER JOIN Products p  ON oi.ProductId = p.Id
                WHERE o.Status = @CompletedStatus
                GROUP BY p.Id, p.ProductType, p.Name, p.ImagePath
                ORDER BY TotalSold DESC
                LIMIT {limit}";
            ;

            var parameters = new Dictionary<string, object>
            {
                { "@CompletedStatus", OrderStatus.Completed.ToString() }
            };

            try
            {
                var reader = (DbDataReader)await ExecuteCommandAsync(query, true, parameters);

                var list = new List<InfoProductTopSalesStatsDTO>();
                try
                {
                    while (await reader.ReadAsync())
                    {
                        list.Add(new InfoProductTopSalesStatsDTO
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            ProductType = System.Enum.Parse<ProductType>(reader.GetString("ProductType")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            ImagePath = reader.IsDBNull("ImagePath") ? null : reader.GetString("ImagePath"),
                            TotalSold = reader.GetInt32(reader.GetOrdinal("TotalSold")),
                        });
                    }
                }
                finally
                {
                    await reader.CloseAsync();
                }

                return list;
            }
            catch (Exception ex)
            {
                Logger.LogError("Greška prilikom dohvatanja top-selling proizvoda.", ex);
                return [];
            }
        }

        public async Task<InfoUserOrderStatsDTO> GetUserOrderStats(int userId, DateTime date)
        {
            const string query = @"
                SELECT

                    (SELECT COUNT(*) FROM Orders WHERE UserId = @UserId) AS TotalOrders,
                    (SELECT COUNT(*) FROM Orders WHERE UserId = @UserId AND Status = 'Completed') AS TotalCompletedOrders,
                    (SELECT COUNT(*) FROM Orders WHERE UserId = @UserId AND Status = 'Cancelled') AS TotalCancelledOrders,
                    (SELECT COUNT(*) FROM Orders WHERE UserId = @UserId AND Status = 'OnHold') AS TotalOnHoldOrders,

                    (SELECT COALESCE(SUM(oi.Quantity), 0)
                        FROM OrderItems oi
                        INNER JOIN Orders o ON oi.OrderId = o.Id
                        WHERE o.UserId = @UserId AND o.Status = 'Completed') AS TotalItems,

                    (SELECT COALESCE(SUM(TotalPrice), 0.0)
                        FROM Orders
                        WHERE UserId = @UserId AND Status = 'Completed') AS TotalRevenue,

                    (SELECT COUNT(*) FROM Orders WHERE UserId = @UserId AND CAST(OrderDate AS DATE) = @Date) AS DailyOrders,
                    (SELECT COUNT(*) FROM Orders WHERE UserId = @UserId AND Status = 'Completed' AND CAST(OrderDate AS DATE) = @Date) AS DailyCompletedOrders,
                    (SELECT COUNT(*) FROM Orders WHERE UserId = @UserId AND Status = 'Cancelled' AND CAST(OrderDate AS DATE) = @Date) AS DailyCancelledOrders,
                    (SELECT COUNT(*) FROM Orders WHERE UserId = @UserId AND Status = 'OnHold' AND CAST(OrderDate AS DATE) = @Date) AS DailyOnHoldOrders,

                    (SELECT COALESCE(SUM(oi.Quantity), 0)
                        FROM OrderItems oi
                        INNER JOIN Orders o ON oi.OrderId = o.Id
                        WHERE o.UserId = @UserId AND o.Status = 'Completed' AND CAST(o.OrderDate AS DATE) = @Date) AS DailyItems,

                    (SELECT COALESCE(SUM(TotalPrice), 0.0)
                        FROM Orders
                        WHERE UserId = @UserId AND Status = 'Completed' AND CAST(OrderDate AS DATE) = @Date) AS DailyRevenue,
       
                    (SELECT COALESCE(SUM(oi.Quantity), 0)
                        FROM OrderItems oi
                        INNER JOIN Orders o ON oi.OrderId   = o.Id
                        INNER JOIN Products p ON oi.ProductId = p.Id
                        WHERE o.UserId = @UserId
                        AND o.Status = 'Completed'
                        AND CAST(o.OrderDate AS DATE) = @Date
                        AND p.ProductType = 'Drink') AS DailyDrinkItems,
 
                    (SELECT COALESCE(SUM(oi.Quantity), 0)
                        FROM OrderItems oi
                        INNER JOIN Orders o ON oi.OrderId   = o.Id
                        INNER JOIN Products p ON oi.ProductId = p.Id
                        WHERE o.UserId = @UserId
                        AND o.Status = 'Completed'
                        AND CAST(o.OrderDate AS DATE) = @Date
                        AND p.ProductType = 'Food') AS DailyFoodItems,

                    (SELECT COALESCE(SUM(oi.Quantity), 0)
                        FROM OrderItems oi
                        INNER JOIN Orders o ON oi.OrderId   = o.Id
                        INNER JOIN Products p ON oi.ProductId = p.Id
                        WHERE o.UserId = @UserId
                        AND o.Status = 'Completed'
                        AND CAST(o.OrderDate AS DATE) = @Date
                        AND p.ProductType = 'Accessory') AS DailyAccessoryItems";

            var parameters = new Dictionary<string, object>
                {
                    { "@UserId", userId },
                    { "@Date", date.Date }
                };

            try
            {
                using var reader = (DbDataReader)await ExecuteCommandAsync(query, true, parameters);

                if (await reader.ReadAsync())
                {
                    return new InfoUserOrderStatsDTO
                    {
                        UserId = userId,
                        TotalOrders = reader.GetInt32(reader.GetOrdinal("TotalOrders")),
                        TotalCompletedOrders = reader.GetInt32(reader.GetOrdinal("TotalCompletedOrders")),
                        TotalCancelledOrders = reader.GetInt32(reader.GetOrdinal("TotalCancelledOrders")),
                        TotalOnHoldOrders = reader.GetInt32(reader.GetOrdinal("TotalOnHoldOrders")),
                        TotalItems = reader.GetInt32(reader.GetOrdinal("TotalItems")),
                        TotalRevenue = reader.GetDecimal(reader.GetOrdinal("TotalRevenue")),
                        DailyOrders = reader.GetInt32(reader.GetOrdinal("DailyOrders")),
                        DailyCompletedOrders = reader.GetInt32(reader.GetOrdinal("DailyCompletedOrders")),
                        DailyCancelledOrders = reader.GetInt32(reader.GetOrdinal("DailyCancelledOrders")),
                        DailyOnHoldOrders = reader.GetInt32(reader.GetOrdinal("DailyOnHoldOrders")),
                        DailyItems = reader.GetInt32(reader.GetOrdinal("DailyItems")),
                        DailyRevenue = reader.GetDecimal(reader.GetOrdinal("DailyRevenue")),
                        DailyDrinkItems = reader.GetInt32(reader.GetOrdinal("DailyDrinkItems")),
                        DailyFoodItems = reader.GetInt32(reader.GetOrdinal("DailyFoodItems")),
                        DailyAccessoryItems = reader.GetInt32(reader.GetOrdinal("DailyAccessoryItems"))
                    };
                }

                return new InfoUserOrderStatsDTO { UserId = userId };
            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška prilikom dohvatanja statistike narudžbi za korisnika {userId}.", ex);
                return new InfoUserOrderStatsDTO { UserId = userId }; ;
            }
        }

        public async Task<InfoOrderStatsDTO> GetOrderStats(DateTime? from, DateTime? to)
        {
            const string sql = @"
                WITH FilteredOrders AS (
                    SELECT *
                    FROM Orders o
                    WHERE (@from IS NULL OR o.OrderDate >= @from)
                        AND (@to   IS NULL OR o.OrderDate <  @to)
                )
                SELECT
                    COUNT(*) AS TotalOrders,
                    SUM(CASE WHEN fo.Status = 'Completed' THEN 1 ELSE 0 END) AS TotalCompletedOrders,
                    SUM(CASE WHEN fo.Status = 'Cancelled' THEN 1 ELSE 0 END) AS TotalCancelledOrders,
                    SUM(CASE WHEN fo.Status = 'OnHold'    THEN 1 ELSE 0 END) AS TotalOnHoldOrders,

                    COALESCE((SELECT SUM(oi.Quantity) 
                                FROM OrderItems oi 
                                WHERE oi.OrderId IN (SELECT Id FROM FilteredOrders)
                                ), 0) AS TotalItems,

                    COALESCE(SUM(CASE WHEN fo.Status = 'Completed' THEN fo.TotalPrice ELSE 0 END), 0.0) AS TotalRevenue,

                    CASE WHEN COUNT(*) > 0 
                            THEN COALESCE(SUM(CASE WHEN fo.Status = 'Completed' THEN fo.TotalPrice ELSE 0 END), 0) / COUNT(*) 
                            ELSE 0 END AS AvgRevenuePerOrder,

                    CASE WHEN COUNT(*) > 0 
                            THEN COALESCE((SELECT SUM(oi.Quantity) 
                                        FROM OrderItems oi 
                                        WHERE oi.OrderId IN (SELECT Id FROM FilteredOrders)
                                        ), 0) 
                                / COUNT(*) 
                            ELSE 0 END AS AvgItemsPerOrder,

                    COALESCE((SELECT SUM(oi.Quantity)
                                FROM OrderItems oi
                                JOIN Products p ON p.Id = oi.ProductId
                                WHERE oi.OrderId IN (SELECT Id FROM FilteredOrders)
                                AND p.ProductType = 'Drink'
                                ), 0) AS DrinkItems,

                    COALESCE((SELECT SUM(oi.Quantity)
                                FROM OrderItems oi
                                JOIN Products p ON p.Id = oi.ProductId
                                WHERE oi.OrderId IN (SELECT Id FROM FilteredOrders)
                                AND p.ProductType = 'Food'
                                ), 0) AS FoodItems,

                    COALESCE((SELECT SUM(oi.Quantity)
                                FROM OrderItems oi
                                JOIN Products p ON p.Id = oi.ProductId
                                WHERE oi.OrderId IN (SELECT Id FROM FilteredOrders)
                                AND p.ProductType = 'Accessory'
                                ), 0) AS AccessoryItems

                FROM FilteredOrders fo;";

            var toAdjusted = to?.Date.AddDays(1);

            var parameters = new Dictionary<string, object>
            {
                { "@from", from.HasValue ? (object) from.Value : DBNull.Value },
                { "@to",   toAdjusted.HasValue ? (object) toAdjusted.Value : DBNull.Value }
            };

            try
            {
                using var reader = (DbDataReader)await ExecuteCommandAsync(sql, true, parameters);
                if (await reader.ReadAsync())
                {
                    return new InfoOrderStatsDTO
                    {
                        TotalOrders = reader.GetInt32(reader.GetOrdinal("TotalOrders")),
                        TotalCompletedOrders = reader.GetInt32(reader.GetOrdinal("TotalCompletedOrders")),
                        TotalCancelledOrders = reader.GetInt32(reader.GetOrdinal("TotalCancelledOrders")),
                        TotalOnHoldOrders = reader.GetInt32(reader.GetOrdinal("TotalOnHoldOrders")),
                        TotalItems = reader.GetInt32(reader.GetOrdinal("TotalItems")),
                        TotalRevenue = reader.GetDecimal(reader.GetOrdinal("TotalRevenue")),
                        AvgRevenuePerOrder = reader.GetDecimal(reader.GetOrdinal("AvgRevenuePerOrder")),
                        AvgItemsPerOrder = reader.GetDouble(reader.GetOrdinal("AvgItemsPerOrder")),
                        DrinkItems = reader.GetInt32(reader.GetOrdinal("DrinkItems")),
                        FoodItems = reader.GetInt32(reader.GetOrdinal("FoodItems")),
                        AccessoryItems = reader.GetInt32(reader.GetOrdinal("AccessoryItems")),
                    };
                }

                return new InfoOrderStatsDTO();
            }
            catch (Exception ex)
            {
                Logger.LogError("Greška prilikom dohvatanja statistika za sve korisnike.", ex);
                return new InfoOrderStatsDTO();
            }
        }

        public async Task<List<InfoWeeklyOrderStatsDTO>> GetUserWeeklyStats(int? userId, int numberOfWeeks = 7)
        {
            const string sql = @"
                WITH RECURSIVE
                    seq AS (SELECT 0 AS n UNION ALL SELECT n + 1 FROM seq WHERE n < @Weeks - 1),
                    WeekList AS (
                        SELECT
                            CAST(DATE_SUB(CURDATE(), INTERVAL WEEKDAY(CURDATE()) DAY) - INTERVAL n WEEK AS DATE) AS WeekStart
                        FROM seq),
                    Stats AS (
                        SELECT
                            CAST(DATE_SUB(CAST(o.OrderDate AS DATE), INTERVAL WEEKDAY(o.OrderDate) DAY) AS DATE) AS WeekStart,
                            COUNT(DISTINCT o.Id) AS TotalOrders,
                            COALESCE(SUM(oi.Quantity), 0) AS TotalItems,
                            COALESCE(SUM(CASE WHEN o.Status = 'Completed' THEN o.TotalPrice ELSE 0 END), 0) AS TotalRevenue
                        FROM Orders o
                        LEFT JOIN OrderItems oi ON oi.OrderId = o.Id
                        WHERE (@UserId IS NULL OR o.UserId = @UserId)
                            AND o.OrderDate >= DATE_SUB(CURDATE(), INTERVAL @Weeks WEEK)
                        GROUP BY WeekStart)
                SELECT
                    wl.WeekStart AS WeekStart,
                    COALESCE(s.TotalOrders, 0) AS TotalOrders,
                    COALESCE(s.TotalItems, 0) AS TotalItems,
                    COALESCE(s.TotalRevenue, 0) AS TotalRevenue
                FROM WeekList wl
                LEFT JOIN Stats s
                    ON s.WeekStart = wl.WeekStart
                ORDER BY wl.WeekStart;";

            var parameters = new Dictionary<string, object>
            {
                { "@UserId", userId.HasValue ? (object)userId.Value : DBNull.Value },
                { "@Weeks", numberOfWeeks }
            };

            try
            {
                using var reader = (DbDataReader)await ExecuteCommandAsync(sql, true, parameters);
                var list = new List<InfoWeeklyOrderStatsDTO>();
                while (await reader.ReadAsync())
                {
                    list.Add(new InfoWeeklyOrderStatsDTO
                    {
                        WeekStart = reader.GetDateTime(reader.GetOrdinal("WeekStart")),
                        TotalOrders = reader.GetInt32(reader.GetOrdinal("TotalOrders")),
                        TotalItems = reader.GetInt32(reader.GetOrdinal("TotalItems")),
                        TotalRevenue = reader.GetDecimal(reader.GetOrdinal("TotalRevenue"))
                    });
                }
                return list;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška prilikom dohvatanja sedmičnih statistika za korisnika {userId}.", ex);
                return [];
            }
        }

        public async Task<int> AddProduct(Product product)
        {
            try
            {
                const string productQuery = @"
                    INSERT INTO Products 
                    (Name, Description, Price, ImagePath, IsAvailable, CreatedDate, UpdatedDate, IsFeatured, Discount, ProductType)
                    VALUES 
                    (@Name, @Description, @Price, @ImagePath, @IsAvailable, @CreatedDate, @UpdatedDate, @IsFeatured, @Discount, @ProductType);
                    SELECT LAST_INSERT_ID();";

                product.IsAvailable = true;
                product.CreatedDate = DateTime.Now;
                product.UpdatedDate = null;
                product.Discount = 0;

                var parameters = new Dictionary<string, object>
                {
                    { "@Name", product.Name },
                    { "@Description", product.Description },
                    { "@Price", product.Price },
                    { "@ImagePath", product.ImagePath ?? (object)DBNull.Value },
                    { "@IsAvailable", product.IsAvailable },
                    { "@CreatedDate", product.CreatedDate },
                    { "@UpdatedDate", product.UpdatedDate ?? (object)DBNull.Value },
                    { "@IsFeatured", product.IsFeatured },
                    { "@Discount", product.Discount },
                    { "@ProductType", product.ProductType.ToString() }
                };

                DbDataReader? reader = null;

                try
                {
                    reader = (DbDataReader)await ExecuteCommandAsync(productQuery, true, parameters);

                    int productId = 0;

                    if (await reader.ReadAsync())
                    {
                        productId = Convert.ToInt32(reader[0]);
                    }
                    await AddProductDetails(product, productId);
                    return productId;
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
                Logger.LogError($"Greška prilikom dodavanja proizvoda {product.Name}.", ex);
                return 0;
            }
        }

        public async Task<int> AddOrder(Order order)
        {
            try
            {
                const string orderQuery = @"
                    INSERT INTO Orders 
                    (UserId, OrderDate, TotalPrice, Status)
                    VALUES 
                    (@UserId, @OrderDate, @TotalPrice, @Status);
                    SELECT LAST_INSERT_ID();";

                var parameters = new Dictionary<string, object>
                {
                    { "@UserId", order.UserId },
                    { "@OrderDate", order.OrderDate },
                    { "@TotalPrice", order.TotalPrice },
                    { "@Status", order.Status.ToString() }
                };

                DbDataReader? reader = null;

                try
                {
                    reader = (DbDataReader)await ExecuteCommandAsync(orderQuery, true, parameters);

                    int orderId = 0;

                    if (await reader.ReadAsync())
                    {
                        orderId = Convert.ToInt32(reader[0]);
                    }
                    return orderId;
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
                Logger.LogError($"Greška prilikom kreiranja narudžbe.", ex);
                return 0;
            }
        }

        public async Task<int> AddOrderItem(OrderItem orderItem)
        {
            try
            {
                const string orderItemQuery = @"
                    INSERT INTO OrderItems
                    (OrderId, ProductId, Quantity, UnitPrice)
                    VALUES 
                    (@OrderId, @ProductId, @Quantity, @UnitPrice);
                    SELECT LAST_INSERT_ID();";

                var parameters = new Dictionary<string, object>
                {
                    { "@OrderId", orderItem.OrderId },
                    { "@ProductId", orderItem.ProductId },
                    { "@Quantity", orderItem.Quantity },
                    { "@UnitPrice", orderItem.UnitPrice }
                };

                DbDataReader? reader = null;

                try
                {
                    reader = (DbDataReader)await ExecuteCommandAsync(orderItemQuery, true, parameters);

                    int orderItemId = 0;

                    if (await reader.ReadAsync())
                    {
                        orderItemId = Convert.ToInt32(reader[0]);
                    }
                    return orderItemId;
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
                Logger.LogError($"Greška prilikom kreiranja stavke narudžbe.", ex);
                return 0;
            }
        }

        public async Task<int> AddPayment(Payment payment)
        {
            try
            {
                const string paymentQuery = @"
                    INSERT INTO Payments
                    (UserId, OrderId, PaymentMethod, PaymentStatus, AmountPaid, CardNumber, PaymentDate)
                    VALUES 
                    (@UserId, @OrderId, @PaymentMethod, @PaymentStatus, @AmountPaid, @CardNumber, @PaymentDate);
                    SELECT LAST_INSERT_ID();";

                var parameters = new Dictionary<string, object>
                {
                    { "@UserId", payment.UserId },
                    { "@OrderId", payment.OrderId },
                    { "@PaymentMethod", payment.PaymentMethod.ToString() },
                    { "@PaymentStatus", payment.PaymentStatus.ToString() },
                    { "@AmountPaid", payment.AmountPaid },
                    { "@CardNumber", payment.CardNumber ?? (object)DBNull.Value },
                    { "@PaymentDate", payment.PaymentDate }
                };

                DbDataReader? reader = null;

                try
                {
                    reader = (DbDataReader)await ExecuteCommandAsync(paymentQuery, true, parameters);

                    int paymentId = 0;

                    if (await reader.ReadAsync())
                    {
                        paymentId = Convert.ToInt32(reader[0]);
                    }
                    return paymentId;
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
                Logger.LogError($"Greška prilikom kreiranja informacije o plaćanju.", ex);
                return 0;
            }
        }

        public async Task<bool> UpdateProductData(Product product)
        {
            try
            {
                const string productQuery = @"
                    UPDATE Products
                    SET Price = @Price,
                        IsFeatured = @IsFeatured,
                        Discount = @Discount,
                        UpdatedDate = @UpdatedDate
                    WHERE Id = @Id;";

                var productParameters = new Dictionary<string, object>
                {
                    { "@Id", product.Id },
                    { "@Price", product.Price },
                    { "@IsFeatured", product.IsFeatured },
                    { "@Discount", product.Discount },
                    { "@UpdatedDate", DateTime.Now }
                };

                var productResult = (int)await ExecuteCommandAsync(productQuery, false, productParameters);

                return productResult > 0;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška prilikom ažuriranja podataka za proizvod za ID: {product.Id}.", ex);
                return false;
            }
        }

        public async Task<bool> UpdateProductProfile(Product product)
        {
            try
            {
                const string productQuery = @"
                    UPDATE Products
                    SET 
                        Name = @Name,
                        Description = @Description,
                        ImagePath = @ImagePath,
                        UpdatedDate = @UpdatedDate
                    WHERE Id = @Id;";

                var parameters = new Dictionary<string, object>
                {
                    { "@Id", product.Id },
                    { "@Name", product.Name },
                    { "@Description", product.Description },
                    { "@ImagePath", product.ImagePath ?? (object)DBNull.Value },
                    { "@UpdatedDate", DateTime.Now }
                };

                var productResult = (int)await ExecuteCommandAsync(productQuery, false, parameters);

                await UpdateProductProfileDetails(product);

                return productResult > 0;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška prilikom ažuriranja profila proizvoda za ID {product.Id}.", ex);
                return false;
            }
        }

        public async Task<bool> UpdateProductAvailability(int productId, bool isAvailable)
        {
            const string query = @"
                UPDATE Products
                SET IsAvailable = @IsAvailable, UpdatedDate = @UpdatedDate
                WHERE Id = @ProductId";

            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@ProductId", productId },
                    { "@IsAvailable", isAvailable },
                    { "@UpdatedDate", DateTime.Now }
                };

                var rowsAffected = (int)await ExecuteCommandAsync(query, false, parameters);

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                Logger.LogError("Greška prilikom ažuriranja dostupnosti proizvoda.", ex);
                return false;
            }
        }

        public async Task<bool> UpdatePayment(Payment payment)
        {
            try
            {
                const string orderQuery = @"
                    UPDATE Payments
                    SET UserId = @UserId,
                        OrderId = @OrderId,
                        PaymentMethod = @PaymentMethod,
                        PaymentStatus = @PaymentStatus,
                        AmountPaid = @AmountPaid,
                        CardNumber = @CardNumber,
                        PaymentDate = @PaymentDate
                    WHERE Id = @Id;";

                var parameters = new Dictionary<string, object>
                {
                    { "@Id", payment.Id },
                    { "@UserId", payment.UserId },
                    { "@OrderId", payment.OrderId },
                    { "@PaymentMethod", payment.PaymentMethod.ToString() },
                    { "@PaymentStatus", payment.PaymentStatus.ToString() },
                    { "@AmountPaid", payment.AmountPaid },
                    { "@CardNumber", payment.CardNumber ?? (object)DBNull.Value },
                    { "@PaymentDate", payment.PaymentDate }
                };

                var orderResult = (int)await ExecuteCommandAsync(orderQuery, false, parameters);

                return orderResult > 0;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška prilikom ažuriranja plaćanja za ID: {payment.Id}.", ex);
                return false;
            }
        }

        public async Task<bool> UpdateOrderData(Order order)
        {
            try
            {
                const string orderQuery = @"
                    UPDATE Orders
                    SET UserId = @UserId,
                        OrderDate = @OrderDate,
                        TotalPrice = @TotalPrice,
                        Status = @Status
                    WHERE Id = @Id;";

                var orderParameters = new Dictionary<string, object>
                {
                    { "@Id", order.Id },
                    { "@UserId", order.UserId },
                    { "@OrderDate", order.OrderDate },
                    { "@TotalPrice", order.TotalPrice },
                    { "@Status", order.Status.ToString() }
                };

                var orderResult = (int)await ExecuteCommandAsync(orderQuery, false, orderParameters);

                return orderResult > 0;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška prilikom ažuriranja podataka za narudžbu za ID: {order.Id}.", ex);
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

        private static async Task<List<Product>> MapReaderToProducts(DbDataReader reader)
        {
            var products = new List<Product>();

            if (reader.IsClosed)
            {
                Logger.LogError("Reader je zatvoren pre mapiranja proizvoda.", new InvalidOperationException("Reader zatvoren"));
                return products;
            }

            while (await reader.ReadAsync())
            {
                if (reader.IsClosed)
                {
                    Logger.LogError("Reader je zatvoren tokom mapiranja proizvoda.", new InvalidOperationException("Reader zatvoren"));
                    return products;
                }
                products.Add(MapReaderToProduct(reader));
            }

            return products;
        }


        private static Product MapReaderToProduct(DbDataReader reader)
        {
            var productType = System.Enum.Parse<ProductType>(reader.GetString("ProductType"));

            return productType switch
            {
                ProductType.Food => MapFoodProduct(reader),
                ProductType.Drink => MapDrinkProduct(reader),
                ProductType.Accessory => MapAccessoryProduct(reader),
                _ => throw new ApplicationException("Nepoznat tip proizvoda.")
            };
        }

        private static FoodProduct MapFoodProduct(DbDataReader reader)
        {
            var foodProduct = new FoodProduct();
            MapBaseProduct(reader, foodProduct);
            foodProduct.FoodType = System.Enum.Parse<FoodType>(reader.GetString("FoodType"));
            foodProduct.Weight = reader.GetDecimal("Weight");
            foodProduct.IsPerishable = reader.GetBoolean("IsPerishable");
            foodProduct.Calories = reader.IsDBNull("Calories") ? null : reader.GetInt32("Calories");
            return foodProduct;
        }

        private static DrinkProduct MapDrinkProduct(DbDataReader reader)
        {
            var drinkProduct = new DrinkProduct();
            MapBaseProduct(reader, drinkProduct);
            drinkProduct.Volume = reader.IsDBNull("Volume") ? null : reader.GetDecimal("Volume");
            drinkProduct.IsAlcoholic = reader.GetBoolean("IsAlcoholic");
            return drinkProduct;
        }

        private static AccessoryProduct MapAccessoryProduct(DbDataReader reader)
        {
            var accessoryProduct = new AccessoryProduct();
            MapBaseProduct(reader, accessoryProduct);
            accessoryProduct.Material = reader.GetString("Material");
            accessoryProduct.Dimensions = reader.IsDBNull("Dimensions") ? null : reader.GetString("Dimensions");
            accessoryProduct.IsReusable = reader.GetBoolean("IsReusable");
            return accessoryProduct;
        }

        private static void MapBaseProduct(DbDataReader reader, Product product)
        {
            product.Id = reader.GetInt32("Id");
            product.ProductType = System.Enum.Parse<ProductType>(reader.GetString("ProductType"));
            product.Name = reader.GetString("Name");
            product.Description = reader.GetString("Description");
            product.Price = reader.GetDecimal("Price");
            product.ImagePath = reader.IsDBNull("ImagePath") ? null : reader.GetString("ImagePath");
            product.IsAvailable = reader.GetBoolean("IsAvailable");
            product.CreatedDate = reader.GetDateTime("CreatedDate");
            product.UpdatedDate = reader.IsDBNull("UpdatedDate") ? null : reader.GetDateTime("UpdatedDate");
            product.IsFeatured = reader.GetBoolean("IsFeatured");
            product.Discount = reader.GetDecimal("Discount");
        }

        private static async Task<List<Order>> MapReaderToOrders(DbDataReader reader)
        {
            var orders = new List<Order>();

            if (reader.IsClosed)
            {
                Logger.LogError("Reader je zatvoren pre mapiranja porudžbina.", new InvalidOperationException("Reader zatvoren"));
                return orders;
            }

            while (await reader.ReadAsync())
            {
                if (reader.IsClosed)
                {
                    Logger.LogError("Reader je zatvoren tokom mapiranja porudžbina.", new InvalidOperationException("Reader zatvoren"));
                    return orders;
                }
                orders.Add(MapOrder(reader));
            }

            return orders;
        }

        private static Order MapOrder(DbDataReader reader)
        {
            return new Order
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                OrderDate = reader.GetDateTime(reader.GetOrdinal("OrderDate")),
                TotalPrice = reader.GetDecimal(reader.GetOrdinal("TotalPrice")),
                Status = System.Enum.Parse<OrderStatus>(reader.GetString("Status"))
            };
        }

        private static async Task<List<OrderItem>> MapReaderToOrderItems(DbDataReader reader)
        {
            var orderItems = new List<OrderItem>();

            if (reader.IsClosed)
            {
                Logger.LogError("Reader je zatvoren pre mapiranja stavki porudžbine.", new InvalidOperationException("Reader zatvoren"));
                return orderItems;
            }

            while (await reader.ReadAsync())
            {
                if (reader.IsClosed)
                {
                    Logger.LogError("Reader je zatvoren tokom mapiranja stavki porudžbine.", new InvalidOperationException("Reader zatvoren"));
                    return orderItems;
                }
                orderItems.Add(MapOrderItem(reader));
            }

            return orderItems;
        }


        private static OrderItem MapOrderItem(DbDataReader reader)
        {
            return new OrderItem
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                OrderId = reader.GetInt32(reader.GetOrdinal("OrderId")),
                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                UnitPrice = reader.GetDecimal(reader.GetOrdinal("UnitPrice"))
            };
        }

        private static Payment MapPayment(DbDataReader reader)
        {
            return new Payment
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                UserId = reader.GetInt32(reader.GetOrdinal("UserId")),
                OrderId = reader.GetInt32(reader.GetOrdinal("OrderId")),
                PaymentMethod = System.Enum.Parse<PaymentMethod>(reader.GetString("PaymentMethod")),
                PaymentStatus = System.Enum.Parse<PaymentStatus>(reader.GetString("PaymentStatus")),
                AmountPaid = reader.GetDecimal(reader.GetOrdinal("AmountPaid")),
                CardNumber = reader.IsDBNull(reader.GetOrdinal("CardNumber"))
                                ? null : reader.GetString(reader.GetOrdinal("CardNumber")),
                PaymentDate = reader.GetDateTime(reader.GetOrdinal("PaymentDate"))
            };
        }

        private async Task AddProductDetails(Product product, int productId)
        {
            string query = product switch
            {
                FoodProduct => @"
                    INSERT INTO FoodProducts 
                    (ProductId, FoodType, Weight, IsPerishable, Calories)
                    VALUES 
                    (@ProductId, @FoodType, @Weight, @IsPerishable, @Calories);",

                DrinkProduct => @"
                    INSERT INTO DrinkProducts 
                    (ProductId, Volume, IsAlcoholic)
                    VALUES 
                    (@ProductId, @Volume, @IsAlcoholic);",

                AccessoryProduct => @"
                    INSERT INTO AccessoryProducts 
                    (ProductId, Material, Dimensions, IsReusable)
                    VALUES 
                    (@ProductId, @Material, @Dimensions, @IsReusable);",

                _ => throw new InvalidOperationException("Nepoznat tip proizvoda.")
            };

            var parameters = new Dictionary<string, object> { { "@ProductId", productId } };

            switch (product)
            {
                case FoodProduct foodProduct:
                    parameters.Add("@FoodType", foodProduct.FoodType.ToString());
                    parameters.Add("@Weight", foodProduct.Weight);
                    parameters.Add("@IsPerishable", foodProduct.IsPerishable);
                    parameters.Add("@Calories", foodProduct.Calories ?? (object)DBNull.Value);
                    break;

                case DrinkProduct drinkProduct:
                    parameters.Add("@Volume", drinkProduct.Volume ?? (object)DBNull.Value);
                    parameters.Add("@IsAlcoholic", drinkProduct.IsAlcoholic);
                    break;

                case AccessoryProduct accessoryProduct:
                    parameters.Add("@Material", accessoryProduct.Material);
                    parameters.Add("@Dimensions", accessoryProduct.Dimensions ?? (object)DBNull.Value);
                    parameters.Add("@IsReusable", accessoryProduct.IsReusable);
                    break;
            }

            await ExecuteCommandAsync(query, false, parameters);
        }

        private async Task UpdateProductProfileDetails(Product product)
        {
            try
            {
                string query = product switch
                {
                    FoodProduct => @"
                        UPDATE FoodProducts 
                        SET 
                            FoodType = @FoodType, 
                            Weight = @Weight, 
                            IsPerishable = @IsPerishable, 
                            Calories = @Calories
                        WHERE ProductId = @ProductId",

                    DrinkProduct => @"
                        UPDATE DrinkProducts 
                        SET 
                            Volume = @Volume, 
                            IsAlcoholic = @IsAlcoholic
                        WHERE ProductId = @ProductId",

                    AccessoryProduct => @"
                        UPDATE AccessoryProducts 
                        SET 
                            Material = @Material, 
                            Dimensions = @Dimensions, 
                            IsReusable = @IsReusable
                        WHERE ProductId = @ProductId",

                    _ => throw new InvalidOperationException("Nepoznat tip proizvoda.")
                };

                var parameters = new Dictionary<string, object> { { "@ProductId", product.Id } };

                switch (product)
                {
                    case FoodProduct foodProduct:
                        parameters.Add("@FoodType", foodProduct.FoodType.ToString());
                        parameters.Add("@Weight", foodProduct.Weight);
                        parameters.Add("@IsPerishable", foodProduct.IsPerishable);
                        parameters.Add("@Calories", foodProduct.Calories ?? (object)DBNull.Value);
                        break;

                    case DrinkProduct drinkProduct:
                        parameters.Add("@Volume", drinkProduct.Volume ?? (object)DBNull.Value);
                        parameters.Add("@IsAlcoholic", drinkProduct.IsAlcoholic);
                        break;

                    case AccessoryProduct accessoryProduct:
                        parameters.Add("@Material", accessoryProduct.Material);
                        parameters.Add("@Dimensions", accessoryProduct.Dimensions ?? (object)DBNull.Value);
                        parameters.Add("@IsReusable", accessoryProduct.IsReusable);
                        break;
                }

                await ExecuteCommandAsync(query, false, parameters);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Greška prilikom ažuriranja detalja za proizvod za ID: {product.Id}.", ex);
            }
        }

    }
}