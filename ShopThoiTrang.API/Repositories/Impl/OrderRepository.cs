using Microsoft.EntityFrameworkCore;
using ShopThoiTrang.API.Data;
using ShopThoiTrang.API.Models;

namespace ShopThoiTrang.API.Repositories.Impl
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
                    .ThenInclude(oi => oi.Product) 
                .FirstOrDefaultAsync(o => o.OrderID == orderId);
        }

        // Lấy danh sách đơn hàng của User (Sắp xếp đơn mới nhất lên đầu)
        public async Task<IEnumerable<Order>> GetOrdersByUserAsync(int userId)
        {
            return await _context.Orders
                .Where(o => o.UserID == userId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.OrderDate) 
                .ToListAsync(); 
        }

        // Lấy tất cả đơn hàng (Cho Admin)
        public async Task<IEnumerable<Order>> GetAllOrdersAsync(string? status)
        {
            var query = _context.Orders
                .Include(o => o.User) 
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