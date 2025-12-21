using Microsoft.EntityFrameworkCore;
using ShopThoiTrang.API.Data;
using ShopThoiTrang.API.Models;

namespace ShopThoiTrang.API.Services
{
    Task<Cart> GetCartAsync(int userId);
    Task<Cart> AddToCartAsync(int userId, int productId, int quantity);
    //jWT
    Task<bool> UpdateItemAsync(int itemId, int quantity);

    //JWT
    Task<bool> RemoveItemAsync(int itemId);

    Task<bool> ClearCartAsync(int userId);
}

public class CartService : ICartService
{
    private readonly AppDbContext _context;

    public CartService(AppDbContext context)
    {
        private readonly AppDbContext _context;

        public CartService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Cart> GetCartAsync(int userId)
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserID == userId && c.IsActive);

            if (cart == null)
            {
                cart = new Cart
                {
                    UserID = userId,
                    CreatedAt = DateTime.Now,
                    IsActive = true
                };

                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }

        public async Task<Cart> AddToCartAsync(int userId, int productId, int quantity)
        {
            var cart = await GetCartAsync(userId);

            var product = await _context.Products.FindAsync(productId);
            if (product == null) throw new Exception("Product not found");

            var item = cart.CartItems.FirstOrDefault(i => i.ProductID == productId);

            if (item != null)
            {
                CartID = cart.CartID,
                ProductID = productId,
                Quantity = quantity,
                UnitPrice = product.Price
            };
            _context.CartItems.Add(item);
        }

        await _context.SaveChangesAsync();
        return await GetCartAsync(userId);
    }
    //JWT
    // 🔥 Cập nhật số lượng
    public async Task<bool> UpdateItemAsync(int itemId, int quantity)
    {
        var item = await _context.CartItems.FindAsync(itemId);
        if (item == null) return false;

            await _context.SaveChangesAsync();
            return await GetCartAsync(userId);
        }

        return true;
    }

    //JWT
    //🔥 Xóa 1 sản phẩm
    public async Task<bool> RemoveItemAsync(int itemId)
    {
        var item = await _context.CartItems.FindAsync(itemId);
        if (item == null) return false;

            var cart = await _context.Carts.FindAsync(item.CartID);
            return await GetCartAsync(cart.UserID);
        }

        public async Task<bool> RemoveItemAsync(int itemId)
        {
            var item = await _context.CartItems.FindAsync(itemId);
            if (item == null) return false;

            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ClearCartAsync(int userId)
        {
            var cart = await GetCartAsync(userId);

            _context.CartItems.RemoveRange(cart.CartItems);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
