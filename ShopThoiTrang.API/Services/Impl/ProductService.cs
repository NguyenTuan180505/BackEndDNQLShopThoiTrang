using ShopThoiTrang.API.Dtos.Products;
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

        public async Task<IEnumerable<ProductViewDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(p => new ProductViewDto
            {
                ProductID = p.ProductID,
                ProductName = p.ProductName,
                Description = p.Description,
                Price = p.Price,
                Discount = p.Discount,
                Stock = p.Stock,
                ImageUrl = p.ImageUrl,
                CreatedAt = p.CreatedAt,
                IsActive = p.IsActive,
                CategoryID = p.CategoryID,
                CategoryName = p.Category.CategoryName
            });
        }

        public async Task<ProductViewDto?> GetByIdAsync(int id)
        {
            var p = await _repo.GetByIdAsync(id);
            if (p == null) return null;

            return new ProductViewDto
            {
                ProductID = p.ProductID,
                ProductName = p.ProductName,
                Description = p.Description,
                Price = p.Price,
                Discount = p.Discount,
                Stock = p.Stock,
                ImageUrl = p.ImageUrl,
                CreatedAt = p.CreatedAt,
                IsActive = p.IsActive,
                CategoryID = p.CategoryID,
                CategoryName = p.Category.CategoryName
            };
        }

        public async Task<ProductViewDto> CreateAsync(ProductCreateDto dto)
        {
            var p = new Product
            {
                ProductName = dto.ProductName,
                Description = dto.Description,
                Price = dto.Price,
                Discount = dto.Discount,
                Stock = dto.Stock,
                ImageUrl = dto.ImageUrl,
                CategoryID = dto.CategoryID
            };

            var created = await _repo.CreateAsync(p);

            return await GetByIdAsync(created.ProductID)
                   ?? throw new Exception("Không thể tạo sản phẩm");
        }

        public async Task<bool> UpdateAsync(int id, ProductUpdateDto dto)
        {
            var p = await _repo.GetByIdAsync(id);
            if (p == null) return false;

            p.ProductName = dto.ProductName;
            p.Description = dto.Description;
            p.Price = dto.Price;
            p.Discount = dto.Discount;
            p.Stock = dto.Stock;
            p.ImageUrl = dto.ImageUrl;
            p.IsActive = dto.IsActive;
            p.CategoryID = dto.CategoryID;

            return await _repo.UpdateAsync(p);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repo.DeleteAsync(id);
        }
    }
}
// update
