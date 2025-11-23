using ShopThoiTrang.API.Models;
using ShopThoiTrang.API.Repositories;

namespace ShopThoiTrang.API.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository; // Inject ProductRepo

        public OrderService(IOrderRepository orderRepository, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _orderRepository.GetAllOrdersAsync(null);
        }

        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _orderRepository.GetOrderByIdAsync(id);
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId)
        {
            return await _orderRepository.GetOrdersByUserAsync(userId);
        }

        // --- LOGIC TẠO ĐƠN HÀNG (Đã sửa theo Product Model của bạn) ---
        public async Task<Order?> CreateOrderAsync(Order order)
        {
            decimal calculatedTotal = 0;

            // Duyệt qua từng sản phẩm khách mua
            foreach (var item in order.OrderItems)
            {
                // 1. Lấy thông tin sản phẩm từ DB
                var product = await _productRepository.GetByIdAsync(item.ProductID);

                if (product == null)
                    throw new Exception($"Sản phẩm ID {item.ProductID} không tồn tại.");

                if (!product.IsActive)
                    throw new Exception($"Sản phẩm {product.ProductName} hiện đang ngừng kinh doanh.");

                // 2. Kiểm tra tồn kho (Sử dụng biến Stock)
                if (product.Stock < item.Quantity)
                    throw new Exception($"Sản phẩm {product.ProductName} không đủ hàng. Chỉ còn {product.Stock}.");

                // 3. Lấy giá từ DB để tính tiền (Bảo mật)
                // Nếu có logic giảm giá (Discount) thì tính ở đây: product.Price - product.Discount
                item.UnitPrice = product.Price;
                calculatedTotal += item.UnitPrice * item.Quantity;

                // 4. Trừ tồn kho
                product.Stock -= item.Quantity;

                // 5. Cập nhật Product vào Context (đánh dấu là đã sửa)
                _productRepository.UpdateProduct(product);
            }

            order.TotalAmount = calculatedTotal;

            // 6. Lưu đơn hàng
            await _orderRepository.AddOrderAsync(order);

            // 7. Commit Transaction (Lưu cả Order và Product cùng lúc)
            // Vì OrderRepo và ProductRepo dùng chung 1 AppDbContext (Scoped) nên chỉ cần SaveChanges 1 lần
            var result = await _orderRepository.SaveChangesAsync();

            return result ? order : null;
        }

        public async Task<bool> UpdateOrderStatusAsync(int id, string status)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);
            if (order == null) return false;

            order.OrderStatus = status;
            _orderRepository.UpdateOrder(order);

            return await _orderRepository.SaveChangesAsync();
        }

        // --- LOGIC HỦY ĐƠN (Hoàn lại Stock) ---
        public async Task<bool> CancelOrderAsync(int id)
        {
            var order = await _orderRepository.GetOrderByIdAsync(id);

            if (order == null) return false;
            if (order.OrderStatus == "Cancelled") return false;

            // Hoàn lại kho cho từng sản phẩm
            foreach (var item in order.OrderItems)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductID);
                if (product != null)
                {
                    product.Stock += item.Quantity; // Cộng lại Stock
                    _productRepository.UpdateProduct(product);
                }
            }

            order.OrderStatus = "Cancelled";
            _orderRepository.UpdateOrder(order);

            return await _orderRepository.SaveChangesAsync();
        }
    }
}