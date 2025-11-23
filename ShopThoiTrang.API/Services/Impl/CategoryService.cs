using ShopThoiTrang.API.Dtos.Categories;
using ShopThoiTrang.API.Models;
using ShopThoiTrang.API.Repositories;

namespace ShopThoiTrang.API.Services.Impl
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;

        public CategoryService(ICategoryRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<CategoryViewDto>> GetAllAsync()
        {
            var list = await _repo.GetAllAsync();
            return list.Select(x => new CategoryViewDto
            {
                CategoryID = x.CategoryID,
                CategoryName = x.CategoryName,
                Description = x.Description
            });
        }

        public async Task<CategoryViewDto?> GetByIdAsync(int id)
        {
            var c = await _repo.GetByIdAsync(id);
            if (c == null) return null;

            return new CategoryViewDto
            {
                CategoryID = c.CategoryID,
                CategoryName = c.CategoryName,
                Description = c.Description
            };
        }

        public async Task<CategoryViewDto> CreateAsync(CategoryCreateDto dto)
        {
            var c = new Category
            {
                CategoryName = dto.CategoryName,
                Description = dto.Description
            };

            var created = await _repo.CreateAsync(c);

            return new CategoryViewDto
            {
                CategoryID = created.CategoryID,
                CategoryName = created.CategoryName,
                Description = created.Description
            };
        }

        public async Task<bool> UpdateAsync(int id, CategoryUpdateDto dto)
        {
            var c = await _repo.GetByIdAsync(id);
            if (c == null) return false;

            c.CategoryName = dto.CategoryName;
            c.Description = dto.Description;

            return await _repo.UpdateAsync(c);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repo.DeleteAsync(id);
        }
    }
}
