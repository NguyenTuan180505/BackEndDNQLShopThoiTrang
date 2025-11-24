using ShopThoiTrang.API.Dtos.Order;
using ShopThoiTrang.API.Models;
using ShopThoiTrang.API.Repositories;
using ShopThoiTrang.API.Services;

namespace ShopThoiTrang.API.Services.Impl
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICartService _cartService;
        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository, ICartService cartService)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _cartService = cartService;
        }
        // 1. TẠO ĐƠN TỪ GIỎ HÀNG
        public async Task<Order> CreateOrderFromCartAsync(int userId, CreateOrderFromCartDto dto)
        {
            var cart = await _cartService.GetCartAsync(userId);

            if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
            {
                throw new Exception("Giỏ hàng trống, không thể tạo đơn.");
            }

            var order = new Order
            {
                UserID = userId,
                OrderDate = DateTime.Now,
                ShippingAddress = dto.ShippingAddress,
                PaymentMethod = dto.PaymentMethod,
                OrderStatus = "Processing",
                PaymentStatus = "Pending",
                OrderItems = new List<OrderItem>()
            };

            decimal totalAmount = 0;

            foreach (var cartItem in cart.CartItems)
            {
                var product = await _productRepository.GetByIdAsync(cartItem.ProductID);

                if (product == null || !product.IsActive)
                    throw new Exception($"Sản phẩm (ID: {cartItem.ProductID}) không khả dụng.");

                if (product.Stock < cartItem.Quantity)
                    throw new Exception($"Sản phẩm '{product.ProductName}' không đủ hàng (Còn: {product.Stock}).");

                var orderItem = new OrderItem
                {
                    ProductID = product.ProductID,
                    Quantity = cartItem.Quantity,
                    UnitPrice = product.Price 
                };

                order.OrderItems.Add(orderItem);
                totalAmount += orderItem.Quantity * orderItem.UnitPrice;

                product.Stock -= cartItem.Quantity;
                await _productRepository.UpdateAsync(product);
            }

            order.TotalAmount = totalAmount;

            await _orderRepository.AddOrderAsync(order);
            await _orderRepository.SaveChangesAsync();

            await _cartService.ClearCartAsync(userId);

            return order;
        }
        // 2. MUA TRỰC TIẾP
        public async Task<Order> CreateOrderDirectAsync(int userId, CreateOrderDirectDto dto)
        {
            if (dto.Items == null || !dto.Items.Any())
                throw new Exception("Danh sách sản phẩm trống.");

            var order = new Order
            {
                UserID = userId,
                OrderDate = DateTime.Now,
                ShippingAddress = dto.ShippingAddress,
                PaymentMethod = dto.PaymentMethod,
                OrderStatus = "Processing",
                PaymentStatus = "Pending",
                OrderItems = new List<OrderItem>()
            };

            decimal totalAmount = 0;

            foreach (var itemDto in dto.Items)
            {
                var product = await _productRepository.GetByIdAsync(itemDto.ProductId);
                if (product == null || !product.IsActive)
                    throw new Exception($"Sản phẩm ID {itemDto.ProductId} không tồn tại.");

                if (product.Stock < itemDto.Quantity)
                    throw new Exception($"Sản phẩm '{product.ProductName}' hết hàng.");
                var orderItem = new OrderItem
                {
                    ProductID = product.ProductID,
                    Quantity = itemDto.Quantity,
                    UnitPrice = product.Price
                };
                order.OrderItems.Add(orderItem);
                totalAmount += orderItem.Quantity * orderItem.UnitPrice;

                product.Stock -= itemDto.Quantity;
                await _productRepository.UpdateAsync(product);
            }

            order.TotalAmount = totalAmount;

            await _orderRepository.AddOrderAsync(order);
            await _orderRepository.SaveChangesAsync();

            return order;
        }

        // 3. LỊCH SỬ ĐƠN HÀNG
        public async Task<IEnumerable<Order>> GetMyOrdersAsync(int userId)
        {
            return await _orderRepository.GetOrdersByUserAsync(userId);
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync() => await _orderRepository.GetAllOrdersAsync(null);
        public async Task<Order?> GetOrderByIdAsync(int id) => await _orderRepository.GetOrderByIdAsync(id);
        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId) => await _orderRepository.GetOrdersByUserAsync(userId);

        // --- TẠO ĐƠN HÀNG
        public async Task<Order?> CreateOrderAsync(Order order)
        {
            decimal calculatedTotal = 0;

            if (order.OrderItems == null || !order.OrderItems.Any())
                throw new Exception("Giỏ hàng trống.");

            foreach (var item in order.OrderItems)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductID);

                if (product == null) throw new Exception($"Sản phẩm ID {item.ProductID} không tồn tại.");
                if (!product.IsActive) throw new Exception($"Sản phẩm '{product.ProductName}' ngừng kinh doanh.");
                if (product.Stock < item.Quantity) throw new Exception($"Sản phẩm '{product.ProductName}' không đủ hàng (Còn {product.Stock}).");

                item.UnitPrice = product.Price;
                calculatedTotal += item.UnitPrice * item.Quantity;
                product.Stock -= item.Quantity;
                await _productRepository.UpdateAsync(product);
            }

            order.TotalAmount = calculatedTotal;
            order.OrderDate = DateTime.Now;

            await _orderRepository.AddOrderAsync(order);

            var result = await _orderRepository.SaveChangesAsync();

            return result ? order : null;
        }

        // --- CẬP NHẬT TRẠNG THÁI ---
        public async Task<bool> UpdateOrderStatusAsync(int id, string status)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null) return false;

            order.OrderStatus = status;
            _orderRepository.UpdateOrder(order);
            return await _orderRepository.SaveChangesAsync();
        }

        // --- HỦY ĐƠN HÀNG (SỬA LẠI ĐỂ GỌI UpdateAsync) ---
        public async Task<bool> CancelOrderAsync(int id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null) return false;
            if (order.OrderStatus == "Cancelled") return false;
            if (order.OrderStatus == "Shipped" || order.OrderStatus == "Delivered")
                throw new Exception("Không thể hủy đơn đang giao hoặc đã giao.");

            if (order.OrderItems != null)
            {
                foreach (var item in order.OrderItems)
                {
                    var product = await _productRepository.GetByIdAsync(item.ProductID);
                    if (product != null)
                    {
                        product.Stock += item.Quantity;
                        await _productRepository.UpdateAsync(product);
                    }
                }
            }
            order.OrderStatus = "Cancelled";
            _orderRepository.UpdateOrder(order);
            return await _orderRepository.SaveChangesAsync();
        }
    }
}