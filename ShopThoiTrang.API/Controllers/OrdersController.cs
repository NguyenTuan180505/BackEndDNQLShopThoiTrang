using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopThoiTrang.API.Dtos.Order;
using ShopThoiTrang.API.Models;
using ShopThoiTrang.API.Services;
using System.Security.Claims;

namespace ShopThoiTrang.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Yêu cầu đăng nhập cho toàn bộ Controller
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        // 1. API TẠO ĐƠN HÀNG (CUSTOMER)

        // POST: api/orders/from-cart (Tạo đơn từ giỏ hàng)
        [HttpPost("from-cart")]
        public async Task<IActionResult> CreateFromCart([FromBody] CreateOrderFromCartDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0) return Unauthorized("Không xác định được người dùng.");

                var order = await _orderService.CreateOrderFromCartAsync(userId, dto);
                
                return Ok(new 
                { 
                    Message = "Đặt hàng từ giỏ thành công!", 
                    OrderId = order.OrderID, 
                    Total = order.TotalAmount 
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // POST: api/orders/direct (Mua ngay / Mua trực tiếp)
        [HttpPost("direct")]
        public async Task<IActionResult> CreateDirect([FromBody] CreateOrderDirectDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0) return Unauthorized("Không xác định được người dùng.");

                var order = await _orderService.CreateOrderDirectAsync(userId, dto);

                return Ok(new 
                { 
                    Message = "Đặt hàng thành công!", 
                    OrderId = order.OrderID, 
                    Total = order.TotalAmount 
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // 2. API XEM ĐƠN HÀNG
       
        // GET: api/orders/my (Lịch sử đơn hàng của tôi)
        [HttpGet("my")] 
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = GetCurrentUserId();
            var orders = await _orderService.GetMyOrdersAsync(userId);

            var orderDtos = orders.Select(MapToResponseDto).ToList();

            return Ok(orderDtos);
        }

        // GET: api/orders/{id} (Chi tiết 1 đơn hàng)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null) return NotFound("Không tìm thấy đơn hàng.");

            var userId = GetCurrentUserId();
            
            if (!IsAdmin() && order.UserID != userId)
            {
                return Forbid();
            }

            return Ok(MapToResponseDto(order));
        }

        // 3. API XỬ LÝ ĐƠN (HỦY / CẬP NHẬT)
       
        // PUT: api/orders/{id}/cancel (Hủy đơn hàng)
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
        {
            try
            {
                var userId = GetCurrentUserId();
                var order = await _orderService.GetOrderByIdAsync(id);

                if (order == null) return NotFound("Không tìm thấy đơn hàng.");

                if (!IsAdmin() && order.UserID != userId)
                {
                    return Forbid();
                }

                var result = await _orderService.CancelOrderAsync(id);

                if (!result)
                    return BadRequest("Không thể hủy đơn hàng này (Đã giao hoặc đã hủy).");

                return Ok(new { message = "Hủy đơn hàng thành công." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/orders/{id}/status (Admin cập nhật trạng thái)
        [HttpPut("{id}/status")]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] OrderUpdateStatusDto dto)
        {
            var result = await _orderService.UpdateOrderStatusAsync(id, dto.OrderStatus);

            if (!result)
                return BadRequest("Cập nhật thất bại hoặc đơn hàng không tồn tại.");

            return Ok(new { message = "Cập nhật trạng thái thành công." });
        }

        // GET: api/orders (Admin xem tất cả)
        [HttpGet]
        [Authorize(Roles = "ADMIN")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            var orderDtos = orders.Select(MapToResponseDto).ToList();
            return Ok(orderDtos);
        }

        // 4. CÁC HÀM PHỤ TRỢ (HELPER)

        private int GetCurrentUserId()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity != null)
            {
                // 1. Token mới dùng "userId"
                var userClaim = identity.FindFirst("UserID");

                // 2. Token cũ dùng "UserID"
                if (userClaim == null)
                    userClaim = identity.FindFirst("UserID");

                // 3. Một số hệ thống dùng NameIdentifier
                if (userClaim == null)
                    userClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (userClaim != null && int.TryParse(userClaim.Value, out int userId))
                    return userId;
            }

            return 0;
        }


        private bool IsAdmin()
        {
            return User.IsInRole("Admin");
        }

        private OrderResponseDto MapToResponseDto(Order order)
        {
            return new OrderResponseDto
            {
                OrderID = order.OrderID,
                UserID = order.UserID,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                ShippingAddress = order.ShippingAddress ?? "N/A",
                PaymentMethod = order.PaymentMethod ?? "N/A",
                PaymentStatus = order.PaymentStatus,
                OrderStatus = order.OrderStatus,
                OrderItems = order.OrderItems?.Select(oi => new OrderItemResponseDto
                {
                    ProductID = oi.ProductID,
                    ProductName = oi.Product?.ProductName ?? "Sản phẩm đã xóa",
                    ProductImage = oi.Product?.ImageUrl ?? "",
                    Quantity = oi.Quantity,
                    UnitPrice = oi.UnitPrice
                }).ToList() ?? new List<OrderItemResponseDto>()
            };
        }
    }
}