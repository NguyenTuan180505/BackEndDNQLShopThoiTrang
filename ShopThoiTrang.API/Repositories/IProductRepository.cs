using ShopThoiTrang.API.Models;

namespace ShopThoiTrang.API.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync(); 

        Task<IEnumerable<Product>> GetAllAsync(string? search, int? categoryId, string? sortBy);

        Task<Product?> GetByIdAsync(int id);

        Task<Product> CreateAsync(Product product);

        Task<bool> UpdateAsync(Product product);

        Task<bool> DeleteAsync(int id);


    }
}