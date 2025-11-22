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
        //public async Task<IActionResult> GetCart()
        //{
        //    int userId = int.Parse(User.FindFirst("UserID").Value);
        //    var cart = await _cartService.GetCartAsync(userId);
        //    return Ok(cart);
        //}

        // GET api/cart/user/1
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetCartByUser(int userId)
        {
            // Lấy giỏ hàng của userId được truyền vào
            var cart = await _cartService.GetCartAsync(userId);

            if (cart == null)
            {
                return Ok(new
                {
                    Message = "Giỏ hàng trống hoặc user không tồn tại",
                    Items = new List<object>(),
                    Total = 0
                });
            }

            return Ok(cart);
        }

        //Jwt
        // POST api/cart/add
        //[HttpPost("add")]
        //public async Task<IActionResult> Add([FromBody] AddCartRequest request)
        //{
        //    int userId = int.Parse(User.FindFirst("UserID").Value);
        //    var cart = await _cartService.AddToCartAsync(userId, request.ProductID, request.Quantity);
        //    return Ok(cart);
        //}

        [HttpPost("add")]
        public async Task<IActionResult> Add([FromBody] AddCartRequest request)
        {
            var cart = await _cartService.AddToCartAsync(request.UserID, request.ProductID, request.Quantity);
            return Ok(cart);
        }

        //Jwt
        // PUT api/cart/update/5
        //[HttpPut("update/{id}")]
        //public async Task<IActionResult> Update(int id, [FromBody] UpdateCartRequest request)
        //{
        //    var cart = await _cartService.UpdateItemAsync(id, request.Quantity);
        //    if (cart == null) return NotFound("Cart item not found");
        //    return Ok(cart);
        //}

        [HttpPut("update/{itemId}")]
        public async Task<IActionResult> Update(int itemId, [FromBody] UpdateCartRequest request)
        {
            var cart = await _cartService.UpdateItemAsync(itemId, request.Quantity, request.UserID);

            if (cart == null)
                return NotFound("Cart item không tồn tại");

            return Ok(cart);
        }

        //Jwt
        // DELETE api/cart/remove/5
        //[HttpDelete("remove/{id}")]
        //public async Task<IActionResult> Remove(int id)
        //{
        //    var result = await _cartService.RemoveItemAsync(id);
        //    return result ? Ok("Removed") : NotFound("Item not found");
        //}

        [HttpDelete("remove/{itemId}/{userId}")]
        public async Task<IActionResult> Remove(int itemId, int userId)
        {
            var result = await _cartService.RemoveItemAsync(itemId, userId);

            return result ? Ok("Removed") : NotFound("Item not found");
        }

        //Jwt
        // DELETE api/cart/clear
        //[HttpDelete("clear")]
        //public async Task<IActionResult> Clear()
        //{
        //    int userId = int.Parse(User.FindFirst("UserID").Value);

        //    await _cartService.ClearCartAsync(userId);

        //    return Ok("Cart cleared");
        //}

        [HttpDelete("clear/{userId}")]
        public async Task<IActionResult> Clear(int userId)
        {
            await _cartService.ClearCartAsync(userId);
            return Ok("Cart cleared");
        }

    }

    //Jwt
    //public class AddCartRequest
    //{
    //    public int ProductID { get; set; }
    //    public int Quantity { get; set; }
    //}

    //public class UpdateCartRequest
    //{
    //    public int Quantity { get; set; }
    //}

    public class AddCartRequest
    {
        public int UserID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
    }

    public class UpdateCartRequest
    {
        public int UserID { get; set; }
        public int Quantity { get; set; }
    }
}
