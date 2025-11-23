using Microsoft.EntityFrameworkCore;
using ShopThoiTrang.API.Data;
using ShopThoiTrang.API.Models;

namespace ShopThoiTrang.API.Repositories.Impl
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AppDbContext _context;

        public ReviewRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Review> CreateAsync(Review review)
        {
            await _context.Reviews.AddAsync(review);
            await _context.SaveChangesAsync();
            return review;
        }

        public async Task<Review?> GetByIdAsync(int id)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.ReviewID == id);
        }

        public async Task<IEnumerable<Review>> GetByProductAsync(int productId)
        {
            return await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.ProductID == productId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();
        }

        public void Delete(Review review)
        {
            _context.Reviews.Remove(review);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
