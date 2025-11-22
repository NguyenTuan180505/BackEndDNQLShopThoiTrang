using ShopThoiTrang.API.Models;
using ShopThoiTrang.API.Repositories;

namespace ShopThoiTrang.API.Services.Impl
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repo;

        public ProductService(IProductRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<Product>> GetAllProducts(string? search, int? categoryId, string? sortBy)
        {
            return await _repo.GetAllAsync(search, categoryId, sortBy);
        }

        public async Task<Product?> GetProductById(int id)
        {
            return await _repo.GetByIdAsync(id);
        }
    }
}
