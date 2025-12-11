using ShopThoiTrang.API.Models;

namespace ShopThoiTrang.API.Repositories
{
    public interface IReviewRepository
    {
        Task<Review> CreateAsync(Review review);
        Task<Review?> GetByIdAsync(int id);

        // Public: review không bị ẩn
        Task<IEnumerable<Review>> GetByProductAsync(int productId);

        // Admin: bao gồm review ẩn
        Task<IEnumerable<Review>> GetAllForProductAdminAsync(int productId);

        void Delete(Review review);
        Task SaveChangesAsync();
    }
}
