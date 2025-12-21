using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopThoiTrang.API.Services;
using System.Security.Claims;

namespace ShopThoiTrang.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        // ================== GET CART ==================
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var cart = await _cartService.GetCartAsync(userId);
            return Ok(cart);
        }

        // ================== ADD ==================
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] AddCartRequest request)
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            var cart = await _cartService.AddToCartAsync(
                userId, request.ProductID, request.Quantity);

            return Ok(cart);
        }

        // ================== UPDATE ==================
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCartRequest request)
        {
            var success = await _cartService.UpdateItemAsync(id, request.Quantity);
            if (!success) return NotFound("Cart item not found");

            return Ok(new
            {
                success = true,
                message = "Cập nhật thành công",
                cartItemId = id,
                quantity = request.Quantity
            });
        }

        // ================== REMOVE ==================
        [HttpDelete("remove/{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            var result = await _cartService.RemoveItemAsync(id);
            return result ? Ok("Removed") : NotFound("Item not found");
        }

        // ================== CLEAR ==================
        [HttpDelete("clear")]
        public async Task<IActionResult> Clear()
        {
            var userId = GetCurrentUserId();
            if (userId == 0) return Unauthorized();

            await _cartService.ClearCartAsync(userId);
            return Ok("Cart cleared");
        }

        // ================== HELPER ==================
        private int GetCurrentUserId()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity != null)
            {
                var claim = identity.FindFirst("UserID")
                            ?? identity.FindFirst(ClaimTypes.NameIdentifier);

                if (claim != null && int.TryParse(claim.Value, out int userId))
                    return userId;
            }

            return 0;
        }
    }

    // ================== REQUEST DTO ==================
    public class AddCartRequest
    {
        public int ProductID { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateCartRequest
    {
        public int Quantity { get; set; }
    }
}
