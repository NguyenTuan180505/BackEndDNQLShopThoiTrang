using ShopThoiTrang.API.Dtos.Categories;

namespace ShopThoiTrang.API.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryViewDto>> GetAllAsync();
        Task<CategoryViewDto?> GetByIdAsync(int id);
        Task<CategoryViewDto> CreateAsync(CategoryCreateDto dto);
        Task<bool> UpdateAsync(int id, CategoryUpdateDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
