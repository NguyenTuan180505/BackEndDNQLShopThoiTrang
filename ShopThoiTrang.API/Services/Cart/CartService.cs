using ShopThoiTrang.API.Models;
using ShopThoiTrang.API.Repositories;

namespace ShopThoiTrang.API.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _repo;

        public CartService(ICartRepository repo)
        {
            _repo = repo;
        }

        public async Task<Cart> GetCartAsync(int userId)
        {
            var cart = await _repo.GetActiveCartAsync(userId);

            if (cart == null)
            {
                cart = await _repo.CreateCartAsync(userId);
            }

            return cart;
        }

        public async Task<Cart> AddToCartAsync(int userId, int productId, int quantity)
        {
            var cart = await GetCartAsync(userId);

            var product = await _repo.GetProductAsync(productId);
            if (product == null)
                throw new Exception("Product not found");

            var item = await _repo.GetCartItemAsync(cart.CartID, productId);

            if (item != null)
            {
                item.Quantity += quantity;
            }
            else
            {
                item = new CartItem
                {
                    CartID = cart.CartID,
                    ProductID = productId,
                    Quantity = quantity,
                    UnitPrice = product.Price
                };
                await _repo.AddCartItemAsync(item);
            }

            await _repo.SaveAsync();
            return await GetCartAsync(userId);
        }

        public async Task<bool> UpdateItemAsync(int itemId, int quantity)
        {
            var item = await _repo.GetCartItemByIdAsync(itemId);
            if (item == null) return false;

            item.Quantity = quantity;
            await _repo.SaveAsync();
            return true;
        }

        public async Task<bool> RemoveItemAsync(int itemId)
        {
            var item = await _repo.GetCartItemByIdAsync(itemId);
            if (item == null) return false;

            await _repo.RemoveCartItemAsync(item);
            await _repo.SaveAsync();
            return true;
        }

        public async Task<bool> ClearCartAsync(int userId)
        {
            var cart = await GetCartAsync(userId);

            await _repo.RemoveCartItemsAsync(cart.CartItems);
            await _repo.SaveAsync();
            return true;
        }
    }
}
