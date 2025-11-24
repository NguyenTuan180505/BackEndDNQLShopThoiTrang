using ShopThoiTrang.API.Dtos.Products;
using ShopThoiTrang.API.Models;

namespace ShopThoiTrang.API.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductViewDto>> GetAllAsync();
        Task<ProductViewDto?> GetByIdAsync(int id);
        Task<ProductViewDto> CreateAsync(ProductCreateDto dto);
        Task<bool> UpdateAsync(int id, ProductUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
// update
