using PastryShop.Model;
using PastryShop.Data.DAO;
using PastryShop.Data.DTO;
using PastryShop.Enum;
using PastryShop.Utility;

namespace PastryShop.Service
{
    public class ProductService(IProductDAO productDao, IStockService stockService) : IProductService
    {
        private readonly IProductDAO _productDao = productDao;
        private readonly IStockService _stockService = stockService;

        public async Task<List<InfoProductBasicDTO>> GetAllProductsBasicInfo()
        {
            var products = await _productDao.GetAllProducts();

            var tasks = products.Select(async product =>
            {
                await _stockService.MarkExpiredAsInactiveAndMarkWarning(product.Id);
                int quantity = await _stockService.GetTotalQuantityForProduct(product.Id);
                return new InfoProductBasicDTO
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    ProductType = product.ProductType,
                    FoodType = product.ProductType == ProductType.Food && product is FoodProduct foodProduct ? foodProduct.FoodType : null,
                    Price = product.Price,
                    Discount = product.Discount,
                    IsFeatured = product.IsFeatured,
                    IsAvailable = product.IsAvailable,
                    ImagePath = product.ImagePath,
                    Quantity = quantity
                };
            });

            var basicInfoProductDTOs = await Task.WhenAll(tasks);
            return [.. basicInfoProductDTOs];
        }

        public async Task<InfoProductDetailsDTO?> GetProductDetailsById(int productId)
        {
            var product = await _productDao.GetProductById(productId);

            if (product == null)
                return null;

            var dto = new InfoProductDetailsDTO
            {
                Id = product.Id,
                ProductType = product.ProductType,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ImagePath = product.ImagePath,
                IsAvailable = product.IsAvailable,
                CreatedDate = product.CreatedDate,
                UpdatedDate = product.UpdatedDate,
                IsFeatured = product.IsFeatured,
                Discount = product.Discount
            };

            if (product is FoodProduct foodProduct)
            {
                dto.FoodType = foodProduct.FoodType;
                dto.Weight = foodProduct.Weight;
                dto.IsPerishable = foodProduct.IsPerishable;
                dto.Calories = foodProduct.Calories;
            }

            if (product is DrinkProduct drinkProduct)
            {
                dto.Volume = drinkProduct.Volume;
                dto.IsAlcoholic = drinkProduct.IsAlcoholic;
            }

            if (product is AccessoryProduct accessoryProduct)
            {
                dto.Material = accessoryProduct.Material;
                dto.Dimensions = accessoryProduct.Dimensions;
                dto.IsReusable = accessoryProduct.IsReusable;
            }

            return dto;
        }

        public async Task<List<String>> GetAllProductNames()
        {
            return await _productDao.GetAllProductNames();
        }

        public async Task<EditProductDataDTO?> GetProductDataForEditById(int productId)
        {
            var product = await _productDao.GetProductById(productId);

            if (product == null)
                return null;

            var dto = new EditProductDataDTO
            {
                Id = product.Id,
                Price = product.Price,
                IsFeatured = product.IsFeatured,
                Discount = product.Discount,
                UpdatedDate = product.UpdatedDate
            };

            return dto;
        }

        public async Task<EditProductProfileDTO?> GetProductProfileForEditById(int productId)
        {
            var product = await _productDao.GetProductById(productId);

            if (product == null)
                return null;

            var dto = new EditProductProfileDTO
            {
                Id = product.Id,
                ProductType = product.ProductType,
                Name = product.Name,
                Description = product.Description,
                ImagePath = product.ImagePath,
                UpdatedDate = product.UpdatedDate
            };

            if (product is FoodProduct foodProduct)
            {
                dto.FoodType = foodProduct.FoodType;
                dto.Weight = foodProduct.Weight;
                dto.IsPerishable = foodProduct.IsPerishable;
                dto.Calories = foodProduct.Calories;
            }

            if (product is DrinkProduct drinkProduct)
            {
                dto.Volume = drinkProduct.Volume;
                dto.IsAlcoholic = drinkProduct.IsAlcoholic;
            }

            if (product is AccessoryProduct accessoryProduct)
            {
                dto.Material = accessoryProduct.Material;
                dto.Dimensions = accessoryProduct.Dimensions;
                dto.IsReusable = accessoryProduct.IsReusable;
            }

            return dto;
        }

        public async Task<List<InfoOrderDTO>> GetRecentOrdersForUser(int userId)
        {
            var orders = await _productDao.GetRecentOrdersForUser(userId);

            var infoOrderDTOs = orders.Select(order => new InfoOrderDTO
            {
                Id = order.Id,
                UserId = order.UserId,
                OrderDate = order.OrderDate,
                TotalPrice = order.TotalPrice,
                Status = order.Status
            }).ToList();

            return infoOrderDTOs;
        }

        public async Task<List<InfoOrderItemDTO>> GetOrderItemsByOrderId(int orderId)
        {
            var orderItems = await _productDao.GetOrderItemsByOrderId(orderId);

            var orderItemsDtosTasks = orderItems.Select(async orderItem =>
            {
                var product = await _productDao.GetProductById(orderItem.ProductId);
                return new InfoOrderItemDTO
                {
                    Id = orderItem.Id,
                    OrderId = orderItem.OrderId,
                    ProductId = orderItem.ProductId,
                    ProductName = product?.Name ?? string.Empty,
                    ProductImagePath = product?.ImagePath ?? string.Empty,
                    Quantity = orderItem.Quantity,
                    UnitPrice = orderItem.UnitPrice,
                    TotalPrice = orderItem.Quantity * orderItem.UnitPrice
                };
            });

            var orderItemsDtos = await Task.WhenAll(orderItemsDtosTasks);
            return [.. orderItemsDtos];
        }

        public async Task<List<InfoProductTopSalesStatsDTO>> GetBestSellersProducts(int limit)
        {
            return await _productDao.GetBestSellersProducts(limit);
        }

        public async Task<InfoUserOrderStatsDTO> GetUserOrderStats(int userId, DateTime date)
        {
            return await _productDao.GetUserOrderStats(userId, date);
        }

        public async Task<InfoOrderStatsDTO> GetOrderStats(DateTime? from, DateTime? to)
        {
            return await _productDao.GetOrderStats(from, to);
        }

        public async Task<List<InfoWeeklyOrderStatsDTO>> GetUserWeeklyStats(int? userId, int numberOfWeeks = 7)
        {
            return await _productDao.GetUserWeeklyStats(userId, numberOfWeeks);
        }

        public async Task CreateProduct(AddProductDTO productDTO)
        {
            if (productDTO == null)
            {
                Logger.LogError("Product DTO je null.", new ArgumentNullException(nameof(productDTO)));
                return;
            }

            Product product = productDTO switch
            {
                AddFoodProductDTO foodProductDTO => new FoodProduct
                {
                    FoodType = foodProductDTO.FoodType ?? default,
                    Weight = foodProductDTO.Weight ?? 0m,
                    IsPerishable = foodProductDTO.IsPerishable ?? true,
                    Calories = foodProductDTO.Calories
                },
                AddDrinkProductDTO drinkProductDTO => new DrinkProduct
                {
                    Volume = drinkProductDTO.Volume,
                    IsAlcoholic = drinkProductDTO.IsAlcoholic ?? false
                },
                AddAccessoryProductDTO accessoryProductDTO => new AccessoryProduct
                {
                    Material = accessoryProductDTO.Material,
                    Dimensions = accessoryProductDTO.Dimensions,
                    IsReusable = accessoryProductDTO.IsReusable ?? false
                },
                _ => throw new ArgumentException("Nepoznat tip proizvoda.")
            };

            product.Name = productDTO.Name;
            product.Description = productDTO.Description;
            product.Price = productDTO.Price;
            product.ImagePath = productDTO.ImagePath;
            product.ProductType = productDTO.ProductType;
            product.IsAvailable = true;
            product.CreatedDate = DateTime.Now;
            product.UpdatedDate = null;
            product.IsFeatured = false;
            product.Discount = 0;

            await _productDao.AddProduct(product);
        }

        public async Task<int> CreateOrder(AddOrderDTO orderDTO)
        {
            if (orderDTO == null)
            {
                Logger.LogError("Order DTO je null.", new ArgumentNullException(nameof(orderDTO)));
                return 0;
            }

            var orderEntity = new Order
            {
                UserId = orderDTO.UserId,
                OrderDate = DateTime.Now,
                TotalPrice = orderDTO.TotalPrice,
                Status = orderDTO.Status
            };

            return await _productDao.AddOrder(orderEntity);
        }

        public async Task<int> CreateOrderItem(AddOrderItemDTO orderItemDTO)
        {
            if (orderItemDTO == null)
            {
                Logger.LogError("Order item DTO je null.", new ArgumentNullException(nameof(orderItemDTO)));
                return 0;
            }

            var orderItemEntity = new OrderItem
            {
                OrderId = orderItemDTO.OrderId,
                ProductId = orderItemDTO.ProductId,
                Quantity = orderItemDTO.Quantity,
                UnitPrice = orderItemDTO.UnitPrice
            };

            return await _productDao.AddOrderItem(orderItemEntity);
        }

        public async Task<int> CreatePayment(AddPaymentDTO paymentDTO)
        {
            if (paymentDTO == null)
            {
                Logger.LogError("Payment DTO je null.", new ArgumentNullException(nameof(paymentDTO)));
                return 0;
            }

            var paymentEntity = new Payment
            {
                UserId = paymentDTO.UserId,
                OrderId = paymentDTO.OrderId,
                PaymentMethod = paymentDTO.PaymentMethod,
                PaymentStatus = paymentDTO.PaymentStatus,
                AmountPaid = paymentDTO.AmountPaid,
                CardNumber = paymentDTO.CardNumber,
                PaymentDate = DateTime.Now
            };

            return await _productDao.AddPayment(paymentEntity);
        }

        public async Task<bool> EditProductData(EditProductDataDTO productDto)
        {
            var existingProduct = await _productDao.GetProductById(productDto.Id);

            if (existingProduct == null)
            {
                Logger.LogError($"Proizvod za ID: {productDto.Id} nije pronađen.", new KeyNotFoundException());
                return false;
            }

            existingProduct.Price = productDto.Price;
            existingProduct.IsFeatured = productDto.IsFeatured;
            existingProduct.Discount = productDto.Discount;
            existingProduct.UpdatedDate = DateTime.Now;

            return await _productDao.UpdateProductData(existingProduct);
        }

        public async Task EditProductProfile(EditProductProfileDTO productDto)
        {
            var existingProduct = await _productDao.GetProductById(productDto.Id);

            if (existingProduct == null)
            {
                Logger.LogError($"Proizvod za ID: {productDto.Id} nije pronađen.", new KeyNotFoundException());
                return;
            }

            existingProduct.Name = productDto.Name;
            existingProduct.Description = productDto.Description;
            if (!string.IsNullOrEmpty(productDto.ImagePath))
            {
                existingProduct.ImagePath = productDto.ImagePath;
            }
            existingProduct.UpdatedDate = DateTime.Now;

            if (existingProduct is FoodProduct foodProduct)
            {
                if (productDto.FoodType.HasValue)
                {
                    foodProduct.FoodType = productDto.FoodType.Value;
                }
                if (productDto.Weight.HasValue)
                {
                    foodProduct.Weight = productDto.Weight.Value;
                }
                if (productDto.IsPerishable.HasValue)
                {
                    foodProduct.IsPerishable = productDto.IsPerishable.Value;
                }
                if (productDto.Calories.HasValue)
                {
                    foodProduct.Calories = productDto.Calories.Value;
                }
            }
            else if (existingProduct is DrinkProduct drinkProduct)
            {
                if (productDto.Volume.HasValue)
                {
                    drinkProduct.Volume = productDto.Volume.Value;
                }
                if (productDto.IsAlcoholic.HasValue)
                {
                    drinkProduct.IsAlcoholic = productDto.IsAlcoholic.Value;
                }
            }
            else if (existingProduct is AccessoryProduct accessoryProduct)
            {
                if (!string.IsNullOrEmpty(productDto.Material))
                {
                    accessoryProduct.Material = productDto.Material;
                }
                if (!string.IsNullOrEmpty(productDto.Dimensions))
                {
                    accessoryProduct.Dimensions = productDto.Dimensions;
                }
                if (productDto.IsReusable.HasValue)
                {
                    accessoryProduct.IsReusable = productDto.IsReusable.Value;
                }
            }
            else
            {
                Logger.LogError("Nepodržan tip proizvoda.", new ArgumentException("Nepodržan tip proizvoda"));
            }

            await _productDao.UpdateProductProfile(existingProduct);
        }

        public async Task<bool> ChangeProductAvailability(int productId, bool isAvailable)
        {
            return await _productDao.UpdateProductAvailability(productId, isAvailable);
        }

        public async Task<bool> EditStatusPayment(EditStatusPaymentDTO paymentDto)
        {
            var existingPayment = await _productDao.GetPaymentByOrderId(paymentDto.OrderId);

            if (existingPayment == null)
            {
                Logger.LogError($"Plaćanje za OrderId: {paymentDto.OrderId} nije pronađeno.", new KeyNotFoundException());
                return false;
            }

            if (paymentDto.PaymentStatus.HasValue)
                existingPayment.PaymentStatus = paymentDto.PaymentStatus.Value;

            existingPayment.PaymentDate = paymentDto.PaymentDate ?? DateTime.Now;

            return await _productDao.UpdatePayment(existingPayment);
        }

        public async Task<bool> EditOrderData(EditOrderDataDTO orderDto)
        {
            var existingOrder = await _productDao.GetOrderById(orderDto.Id);

            if (existingOrder == null)
            {
                Logger.LogError($"Narudžba za ID: {orderDto.Id} nije pronađena.", new KeyNotFoundException());
                return false;
            }

            if (orderDto.UserId.HasValue)
                existingOrder.UserId = orderDto.UserId.Value;
            if (orderDto.OrderDate.HasValue)
                existingOrder.OrderDate = orderDto.OrderDate.Value;
            if (orderDto.TotalPrice.HasValue)
                existingOrder.TotalPrice = orderDto.TotalPrice.Value;
            if (orderDto.Status.HasValue)
                existingOrder.Status = orderDto.Status.Value;

            return await _productDao.UpdateOrderData(existingOrder);
        }

    }
}