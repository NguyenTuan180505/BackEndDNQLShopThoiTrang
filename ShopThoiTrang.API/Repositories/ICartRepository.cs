using ShopThoiTrang.API.Models;

namespace ShopThoiTrang.API.Repositories
{
    public interface ICartRepository
    {
        Task<Cart?> GetActiveCartAsync(int userId);
        Task<Cart> CreateCartAsync(int userId);

        Task<Product?> GetProductAsync(int productId);

        Task<CartItem?> GetCartItemAsync(int cartId, int productId);
        Task<CartItem?> GetCartItemByIdAsync(int itemId);

        Task AddCartItemAsync(CartItem item);
        Task SaveAsync();

        Task RemoveCartItemAsync(CartItem item);
        Task RemoveCartItemsAsync(IEnumerable<CartItem> items);
    }
}
