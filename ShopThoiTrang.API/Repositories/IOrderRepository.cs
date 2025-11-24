using ShopThoiTrang.API.Models;

namespace ShopThoiTrang.API.Repositories
{
    public interface IOrderRepository
    {
        Task<Order?> GetOrderByIdAsync(int orderId);
        Task<IEnumerable<Order>> GetOrdersByUserAsync(int userId);
        Task<IEnumerable<Order>> GetAllOrdersAsync(string? status);
        Task AddOrderAsync(Order order);
        void UpdateOrder(Order order); // Mới thêm
        Task<bool> SaveChangesAsync();

    }
}