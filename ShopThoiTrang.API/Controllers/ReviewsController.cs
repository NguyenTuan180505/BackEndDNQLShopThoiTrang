using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopThoiTrang.API.Dtos.Review;
using ShopThoiTrang.API.Services;
using System.Security.Claims;

namespace ShopThoiTrang.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        private bool IsAdmin()
        {
            return User.IsInRole("Admin");
        }

        // ========================================================
        // 1) Xem review theo Product + lọc rating (Public/Admin)
        // ========================================================
        // Ví dụ:
        // GET /api/reviews/product/10?rating=5
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetByProduct(int productId, [FromQuery] int? rating)
        {
            bool isAdmin = IsAdmin();
            var data = await _reviewService.GetByProductAsync(productId, isAdmin, rating);
            return Ok(data);
        }

        // ========================================================
        // 2) Customer tạo review
        // ========================================================
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create(CreateReviewDto dto)
        {
            int userId = GetUserId();
            var result = await _reviewService.CreateAsync(dto, userId);
            return Ok(result);
        }

        // ========================================================
        // 3) Customer xoá review của họ / Admin xoá tất cả
        // ========================================================
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            int userId = GetUserId();
            bool isAdmin = IsAdmin();

            var ok = await _reviewService.DeleteAsync(id, userId, isAdmin);
            if (!ok) return NotFound();

            return NoContent();
        }

        // ========================================================
        // 4) ADMIN ẨN REVIEW
        // ========================================================
        [HttpPatch("{id}/hide")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> HideReview(int id)
        {
            bool ok = await _reviewService.HideAsync(id);
            if (!ok) return NotFound();

            return Ok(new { message = "Review đã được ẩn." });
        }
    }
}
