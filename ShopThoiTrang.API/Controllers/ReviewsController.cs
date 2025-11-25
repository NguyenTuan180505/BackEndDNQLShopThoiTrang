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
        private readonly IUserService _userService;

        public ReviewsController(IReviewService reviewService, IUserService userService)
        {
            _reviewService = reviewService;
            _userService = userService;
        }

        // ⭐ Lấy userId từ claim "userId"
        private int GetUserId()
        {
            var id = User.FindFirstValue("userId");

            if (string.IsNullOrEmpty(id))
                throw new UnauthorizedAccessException("JWT does not contain 'userId' claim.");

            return int.Parse(id);
        }


        // ⭐ Kiểm tra role dựa trên JwtService (ADMIN hoặc CUSTOMER)
        private bool IsAdmin() => User.IsInRole("ADMIN");
        private bool IsCustomer() => User.IsInRole("CUSTOMER");

        // GET: /api/reviews/product/{productId}
        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetByProduct(int productId, [FromQuery] int? rating)
        {
            bool isAdmin = IsAdmin();
            var data = await _reviewService.GetByProductAsync(productId, isAdmin, rating);
            return Ok(data);
        }

        // ⭐ ALLOW CUSTOMER đúng theo JWT (Customer)
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create(CreateReviewDto dto)
        {
            int userId = GetUserId();
            var result = await _reviewService.CreateAsync(dto, userId);
            return Ok(result);
        }

        // ⭐ CUSTOMER hoặc ADMIN
        [HttpDelete("{id}")]
        [Authorize(Roles = "Customer,Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            int userId = GetUserId();
            bool isAdmin = IsAdmin();

            var ok = await _reviewService.DeleteAsync(id, userId, isAdmin);
            if (!ok) return NotFound();

            return NoContent();
        }

        // ⭐ ADMIN
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
