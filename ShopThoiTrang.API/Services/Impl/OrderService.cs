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

        public OrderService(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            ICartService cartService)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _cartService = cartService;
        }

        // 1. HÀM TẠO ĐƠN DUY NHẤT (GỘP CART & DIRECT)
      
        public async Task<Order> CreateOrderAsync(int userId, OrderCreateDto dto)
        {
            if (dto.Items == null || !dto.Items.Any())
            {
                throw new Exception("Danh sách sản phẩm trống. Vui lòng chọn sản phẩm.");
            }

            // 2. Khởi tạo đơn hàng
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

                if (product == null)
                    throw new Exception($"Sản phẩm ID {itemDto.ProductId} không tồn tại.");

                if (!product.IsActive)
                    throw new Exception($"Sản phẩm '{product.ProductName}' hiện đang ngừng kinh doanh.");

                if (product.Stock < itemDto.Quantity)
                    throw new Exception($"Sản phẩm '{product.ProductName}' không đủ hàng (Còn: {product.Stock}).");

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

            if (dto.IsFromCart)
            {
                await _cartService.ClearCartAsync(userId);
            }

            return order;
        }

        // 2. CÁC HÀM GET DỮ LIỆU
        
        public async Task<IEnumerable<Order>> GetAllOrdersAsync() => await _orderRepository.GetAllOrdersAsync(null);
        public async Task<Order?> GetOrderByIdAsync(int id) => await _orderRepository.GetOrderByIdAsync(id);
        public async Task<IEnumerable<Order>> GetMyOrdersAsync(int userId) => await _orderRepository.GetOrdersByUserAsync(userId);

        // 3. CÁC HÀM XỬ LÝ TRẠNG THÁI (UPDATE / CANCEL)
        
        public async Task<bool> UpdateOrderStatusAsync(int id, string status)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null) return false;

            order.OrderStatus = status;
            _orderRepository.UpdateOrder(order);
            return await _orderRepository.SaveChangesAsync();
        }

        public async Task<bool> CancelOrderAsync(int id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null) return false;

            if (order.OrderStatus == "Cancelled") return false;
            if (order.OrderStatus == "Shipped" || order.OrderStatus == "Delivered")
                throw new Exception("Không thể hủy đơn hàng đang giao hoặc đã giao thành công.");

            // Hoàn lại kho
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