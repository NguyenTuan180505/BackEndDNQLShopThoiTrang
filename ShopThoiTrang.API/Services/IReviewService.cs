using ShopThoiTrang.API.Dtos.Review;

namespace ShopThoiTrang.API.Services
{
    public interface IReviewService
    {
        Task<ReviewDto> CreateAsync(CreateReviewDto dto, int userId);
        Task<IEnumerable<ReviewDto>> GetByProductAsync(int productId);
        Task<bool> DeleteAsync(int id, int userId);
    }
}
