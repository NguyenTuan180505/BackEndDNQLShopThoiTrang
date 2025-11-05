using ShopThoiTrang.API.Models;

namespace ShopThoiTrang.API.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProducts(string? search, int? categoryId, string? sortBy);
        Task<Product?> GetProductById(int id);
    }
}
