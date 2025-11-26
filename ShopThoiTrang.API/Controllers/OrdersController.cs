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

        // 1. API TẠO ĐƠN HÀNG (GỘP CHUNG)
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0) return Unauthorized("Không xác định được người dùng. Vui lòng đăng nhập lại.");

                var order = await _orderService.CreateOrderAsync(userId, dto);

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
        [Authorize(Roles = "ADMIN")]  // <-- ĐÚNG (Phải viết hoa giống DB)
        [HttpGet]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            var orderDtos = orders.Select(MapToResponseDto).ToList();
            return Ok(orderDtos);
        }

        private bool IsAdmin()
        {
            return User.IsInRole("ADMIN");
        }
        // 4. CÁC HÀM PHỤ TRỢ (PRIVATE HELPER)
        private int GetCurrentUserId()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                // 1. Ưu tiên tìm key "userId" (Khớp với JwtService của bạn)
                var claim = identity.FindFirst("userId");

                // 2. Fallback: Tìm các chuẩn khác
                if (claim == null) claim = identity.FindFirst(ClaimTypes.NameIdentifier);
                if (claim == null) claim = identity.FindFirst("UserID");
                if (claim == null) claim = identity.FindFirst("Id");

                if (claim != null && int.TryParse(claim.Value, out int userId))
                {
                    return userId;
                }
            }
            return 0;
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