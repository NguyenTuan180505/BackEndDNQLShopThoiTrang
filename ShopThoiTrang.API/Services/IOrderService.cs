using ShopThoiTrang.API.Models;

namespace ShopThoiTrang.API.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId);

        Task<Order?> GetOrderByIdAsync(int id);

        Task<Order?> CreateOrderAsync(Order order);

        Task<bool> UpdateOrderStatusAsync(int id, string status);

        Task<bool> CancelOrderAsync(int id);

    }
}