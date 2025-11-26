using ShopThoiTrang.API.Dtos.Order;
using ShopThoiTrang.API.Models;

namespace ShopThoiTrang.API.Services
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order?> GetOrderByIdAsync(int id);
        Task<IEnumerable<Order>> GetMyOrdersAsync(int userId);
        Task<bool> UpdateOrderStatusAsync(int id, string status);
        Task<bool> CancelOrderAsync(int id);

        Task<Order> CreateOrderAsync(int userId, OrderCreateDto dto);
    }
}