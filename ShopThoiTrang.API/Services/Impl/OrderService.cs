using ShopThoiTrang.API.Models;
using ShopThoiTrang.API.Repositories;

namespace ShopThoiTrang.API.Services.Impl
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;

        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        // ... Các hàm Get (GetAllOrdersAsync, GetById...) giữ nguyên ...
        public async Task<IEnumerable<Order>> GetAllOrdersAsync() => await _orderRepository.GetAllOrdersAsync(null);
        public async Task<Order?> GetOrderByIdAsync(int id) => await _orderRepository.GetOrderByIdAsync(id);
        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId) => await _orderRepository.GetOrdersByUserAsync(userId);

        // --- TẠO ĐƠN HÀNG (SỬA LẠI ĐỂ GỌI UpdateAsync) ---
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

                // Trừ kho
                product.Stock -= item.Quantity;

                // SỬA Ở ĐÂY: Gọi UpdateAsync thay vì UpdateProduct
                await _productRepository.UpdateAsync(product);
            }

            order.TotalAmount = calculatedTotal;
            order.OrderDate = DateTime.Now;

            await _orderRepository.AddOrderAsync(order);

            // Lưu Order (Product đã được save trong hàm UpdateAsync ở trên rồi, nhưng gọi SaveChanges ở đây để chốt Order)
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

            // Hoàn kho
            if (order.OrderItems != null)
            {
                foreach (var item in order.OrderItems)
                {
                    var product = await _productRepository.GetByIdAsync(item.ProductID);
                    if (product != null)
                    {
                        product.Stock += item.Quantity;

                        // SỬA Ở ĐÂY: Gọi UpdateAsync thay vì UpdateProduct
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