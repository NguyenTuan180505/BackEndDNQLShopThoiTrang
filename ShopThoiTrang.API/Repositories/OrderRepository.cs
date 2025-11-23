using Microsoft.EntityFrameworkCore;
using ShopThoiTrang.API.Data;
using ShopThoiTrang.API.Models;

namespace ShopThoiTrang.API.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        // Lấy chi tiết 1 đơn hàng
        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product) // Include sâu vào Product để lấy tên/ảnh
                                                   // .Include(o => o.Payments) // Bỏ comment nếu có bảng Payment
                .FirstOrDefaultAsync(o => o.OrderID == orderId);
        }

        // Lấy danh sách đơn hàng của User (Sắp xếp đơn mới nhất lên đầu)
        public async Task<IEnumerable<Order>> GetOrdersByUserAsync(int userId)
        {
            return await _context.Orders
                .Where(o => o.UserID == userId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.OrderDate) // Sắp xếp giảm dần ngày tạo
                .ToListAsync(); // Thực thi query ngay tại đây
        }

        // Lấy tất cả đơn hàng (Cho Admin)
        public async Task<IEnumerable<Order>> GetAllOrdersAsync(string? status)
        {
            var query = _context.Orders
                .Include(o => o.User) // Lấy thông tin người mua
                .Include(o => o.OrderItems)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(o => o.OrderStatus == status);
            }

            return await query.OrderByDescending(o => o.OrderDate).ToListAsync();
        }

        // Tạo đơn hàng mới
        public async Task AddOrderAsync(Order order)
        {
            // Chỉ cần Add Order, EF Core tự động Add luôn OrderItems bên trong nó
            await _context.Orders.AddAsync(order);
        }

        // Cập nhật đơn hàng (Dùng cho chức năng đổi Status, Cancel)
        public void UpdateOrder(Order order)
        {
            _context.Orders.Update(order);
        }

        // Lưu thay đổi
        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}