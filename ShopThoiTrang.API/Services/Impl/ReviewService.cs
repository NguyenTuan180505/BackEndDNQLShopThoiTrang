using ShopThoiTrang.API.Data;
using ShopThoiTrang.API.Dtos.Review;
using ShopThoiTrang.API.Models;
using ShopThoiTrang.API.Repositories;

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

        // ============================================
        // 1) Tạo review (Customer)
        // ============================================
        public async Task<ReviewDto> CreateAsync(CreateReviewDto dto, int userId)
        {
            var review = new Review
            {
                ProductID = dto.ProductID,
                UserID = userId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                CreatedAt = DateTime.Now,
                IsHidden = false,
                ImageUrls = dto.ImageUrls ?? new List<string>(),
                VideoUrls = dto.VideoUrls ?? new List<string>()
            };

            var created = await _reviewRepo.CreateAsync(review);

            created = await _reviewRepo.GetByIdAsync(created.ReviewID);

            return MapToDto(created!);
        }

        // ============================================
        // 2) Lấy review theo product (Public / Admin + Lọc rating)
        // ============================================
        public async Task<IEnumerable<ReviewDto>> GetByProductAsync(int productId, bool isAdmin, int? rating)
        {
            IEnumerable<Review> list = isAdmin
                ? await _reviewRepo.GetAllForProductAdminAsync(productId)
                : await _reviewRepo.GetByProductAsync(productId);

            if (rating.HasValue)
                list = list.Where(r => r.Rating == rating.Value);

            return list.Select(MapToDto);
        }

        // ============================================
        // 3) Xóa review (Customer xóa của họ, Admin xóa tất cả)
        // ============================================
        public async Task<bool> DeleteAsync(int id, int userId, bool isAdmin)
        {
            var review = await _reviewRepo.GetByIdAsync(id);
            if (review == null) return false;

            if (!isAdmin && review.UserID != userId)
                throw new Exception("Bạn không thể xóa đánh giá của người khác.");

            _reviewRepo.Delete(review);
            await _reviewRepo.SaveChangesAsync();

            return true;
        }

        // ============================================
        // 4) ẨN review (Chỉ Admin)
        // ============================================
        public async Task<bool> HideAsync(int id)
        {
            var review = await _reviewRepo.GetByIdAsync(id);
            if (review == null) return false;

            review.IsHidden = true;
            await _reviewRepo.SaveChangesAsync();
            return true;
        }

        // ============================================
        // Mapping DTO
        // ============================================
        private ReviewDto MapToDto(Review r)
        {
            return new ReviewDto
            {
                ReviewID = r.ReviewID,
                ProductID = r.ProductID,
                UserID = r.UserID,
                FullName = r.User?.FullName ?? "Unknown",
                Rating = r.Rating,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt,
                IsHidden = r.IsHidden,
                ImageUrls = r.ImageUrls ?? new List<string>(),
                VideoUrls = r.VideoUrls ?? new List<string>()
            };
        }
    }
}
