using ShopThoiTrang.API.Dtos.Review;

namespace ShopThoiTrang.API.Services
{
    public interface IReviewService
    {
        Task<ReviewDto> CreateAsync(CreateReviewDto dto, int userId);

        Task<IEnumerable<ReviewDto>> GetByProductAsync(
            int productId,
            bool isAdmin,
            int? rating
        );

        Task<bool> DeleteAsync(int id, int userId, bool isAdmin);

        Task<bool> HideAsync(int id);  // Admin only
    }
}
