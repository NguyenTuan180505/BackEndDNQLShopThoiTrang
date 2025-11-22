using ShopThoiTrang.API.Models;

namespace ShopThoiTrang.API.Repositories
{
    public interface IReviewRepository
    {
        Task<Review> CreateAsync(Review review);
        Task<Review?> GetByIdAsync(int id);
        Task<IEnumerable<Review>> GetByProductAsync(int productId);
        void Delete(Review review);
        Task SaveChangesAsync();
    }
}
