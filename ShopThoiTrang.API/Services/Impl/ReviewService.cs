using ShopThoiTrang.API.Data;
using ShopThoiTrang.API.Dtos.Review;
using ShopThoiTrang.API.Models;
using ShopThoiTrang.API.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ShopThoiTrang.API.Services.Impl
{
    public class ReviewService : IReviewService
    {
        private readonly AppDbContext _context;
        private readonly IReviewRepository _reviewRepo;

        public ReviewService(AppDbContext context, IReviewRepository reviewRepo)
        {
            _context = context;
            _reviewRepo = reviewRepo;
        }

        public async Task<ReviewDto> CreateAsync(CreateReviewDto dto, int userId)
        {
            var review = new Review
            {
                ProductID = dto.ProductID,
                UserID = userId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                CreatedAt = DateTime.Now
            };

            var created = await _reviewRepo.CreateAsync(review);

            created = await _reviewRepo.GetByIdAsync(created.ReviewID);

            return MapToDto(created!);
        }

        public async Task<IEnumerable<ReviewDto>> GetByProductAsync(int productId)
        {
            var list = await _reviewRepo.GetByProductAsync(productId);
            return list.Select(MapToDto);
        }

        public async Task<bool> DeleteAsync(int id, int userId)
        {
            var review = await _reviewRepo.GetByIdAsync(id);
            if (review == null) return false;

            if (review.UserID != userId)
                throw new Exception("Bạn không thể xóa bài đánh giá của người khác!");

            _reviewRepo.Delete(review);
            await _reviewRepo.SaveChangesAsync();
            return true;
        }

        private ReviewDto MapToDto(Review r)
        {
            return new ReviewDto
            {
                ReviewID = r.ReviewID,
                ProductID = r.ProductID,
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt,
                FullName = r.User?.FullName ?? "Unknown"
            };
        }

    }
}
