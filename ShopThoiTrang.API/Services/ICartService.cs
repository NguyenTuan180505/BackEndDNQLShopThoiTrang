using ShopThoiTrang.API.Models;

namespace ShopThoiTrang.API.Services
{
    public interface ICartService
    {
        Task<Cart> GetCartAsync(int userId);
        Task<Cart> AddToCartAsync(int userId, int productId, int quantity);
        Task<Cart?> UpdateItemAsync(int itemId, int quantity);
        Task<bool> RemoveItemAsync(int itemId);
        Task<bool> ClearCartAsync(int userId);
    }
}
