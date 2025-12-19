using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace ShopThoiTrang.API.Controllers
{

    //private readonly AppDbContext context;
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Customer")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }
        //Jwt
        //GET api/cart
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            int userId = int.Parse(User.FindFirst("UserID").Value);
            var cart = await _cartService.GetCartAsync(userId);
            return Ok(cart);
        }



        //Jwt
        // POST api/cart/add
        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] AddCartRequest request)
        {
            int userId = int.Parse(User.FindFirst("UserID").Value);
            var cart = await _cartService.AddToCartAsync(userId, request.ProductID, request.Quantity);
            return Ok(cart);
        }


        //Jwt
        // PUT api/cart/update/5
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


        //Jwt
        // DELETE api/cart/remove/5
        [HttpDelete("remove/{id}")]
        public async Task<IActionResult> Remove(int id)
        {
            var result = await _cartService.RemoveItemAsync(id);
            return result ? Ok("Removed") : NotFound("Item not found");
        }

        //Jwt
        // DELETE api/cart/clear
        [HttpDelete("clear")]
        public async Task<IActionResult> Clear()
        {
            int userId = int.Parse(User.FindFirst("UserID").Value);

            await _cartService.ClearCartAsync(userId);

            return Ok("Cart cleared");
        }


    }

    //Jwt
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
