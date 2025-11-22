using Microsoft.EntityFrameworkCore;
using ShopThoiTrang.API.Data;
using ShopThoiTrang.API.Models;

public interface ICartService
{
    Task<Cart> GetCartAsync(int userId);
    Task<Cart> AddToCartAsync(int userId, int productId, int quantity);
    Task<Cart> UpdateItemAsync(int itemId, int quantity);
    Task<bool> RemoveItemAsync(int itemId);
    Task<bool> ClearCartAsync(int userId);
}

public class CartService : ICartService
{
    private readonly AppDbContext _context;

    public CartService(AppDbContext context)
    {
        _context = context;
    }

    // 🔥 Lấy giỏ hoặc tạo mới
    public async Task<Cart> GetCartAsync(int userId)
    {
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
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

    // 🔥 Thêm sản phẩm (kiểm tra trùng)
    public async Task<Cart> AddToCartAsync(int userId, int productId, int quantity)
    {
        var cart = await GetCartAsync(userId);

        var product = await _context.Products.FindAsync(productId);
        if (product == null) throw new Exception("Product not found");

        var item = cart.CartItems?.FirstOrDefault(i => i.ProductID == productId);

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
            _context.CartItems.Add(item);
        }

        await _context.SaveChangesAsync();
        return await GetCartAsync(userId);
    }

    // 🔥 Cập nhật số lượng
    public async Task<Cart> UpdateItemAsync(int itemId, int quantity)
    {
        var item = await _context.CartItems.FindAsync(itemId);
        if (item == null) return null;

        item.Quantity = quantity;
        await _context.SaveChangesAsync();

        return await GetCartAsync(item.CartID);
    }

    // 🔥 Xóa 1 sản phẩm
    public async Task<bool> RemoveItemAsync(int itemId)
    {
        var item = await _context.CartItems.FindAsync(itemId);
        if (item == null) return false;

        _context.CartItems.Remove(item);
        await _context.SaveChangesAsync();
        return true;
    }

    // 🔥 Xóa toàn bộ giỏ
    public async Task<bool> ClearCartAsync(int userId)
    {
        var cart = await GetCartAsync(userId);

        _context.CartItems.RemoveRange(cart.CartItems);
        await _context.SaveChangesAsync();

        return true;
    }
}
