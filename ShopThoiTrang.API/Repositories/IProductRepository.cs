using ShopThoiTrang.API.Models;

namespace ShopThoiTrang.API.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync(string? search, int? categoryId, string? sortBy);
        Task<Product?> GetByIdAsync(int id);

        Task AddAsync(Product product);

        // SỬA: UpdateProductAsync -> UpdateProduct (void)
        void UpdateProduct(Product product);

        // SỬA: DeleteProductAsync -> DeleteProduct (void)
        void DeleteProduct(Product product);

        // THÊM: Hàm này trong Class có mà Interface thiếu
        Task<bool> SaveChangesAsync();
    }
}
