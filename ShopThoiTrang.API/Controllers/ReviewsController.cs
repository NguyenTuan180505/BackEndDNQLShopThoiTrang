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

        // Lấy userId đúng theo token bạn đã tạo
        private int GetUserId()
        {
            var claim = User.FindFirst("userId")?.Value;
            if (claim == null)
                throw new UnauthorizedAccessException("Token không chứa userId.");

            return int.Parse(claim);
        }

        [HttpGet("product/{productId}")]
        public async Task<IActionResult> GetByProduct(int productId)
        {
            var data = await _reviewService.GetByProductAsync(productId);
            return Ok(data);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create(CreateReviewDto dto)
        {
            int userId = GetUserId();

            var result = await _reviewService.CreateAsync(dto, userId);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            int userId = GetUserId();

            var ok = await _reviewService.DeleteAsync(id, userId);
            if (!ok) return NotFound();

            return NoContent();
        }
    }
}
