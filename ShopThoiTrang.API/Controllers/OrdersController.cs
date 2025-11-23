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

        // 1. API CHO KHÁCH HÀNG (CUSTOMER)
        // POST: api/orders (Tạo đơn hàng mới)
        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDto createDto)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == 0) return Unauthorized("Vui lòng đăng nhập lại.");

                var newOrder = new Order
                {
                    UserID = userId,
                    ShippingAddress = createDto.ShippingAddress,
                    PaymentMethod = createDto.PaymentMethod,
                    PaymentStatus = "Pending",   
                    OrderStatus = "Processing",  
                    OrderDate = DateTime.Now,

                    OrderItems = createDto.OrderItems.Select(item => new OrderItem
                    {
                        ProductID = item.ProductID,
                        Quantity = item.Quantity
                    }).ToList()
                };

                var createdOrder = await _orderService.CreateOrderAsync(newOrder);

                if (createdOrder == null)
                {
                    return BadRequest("Không thể tạo đơn hàng. Vui lòng thử lại.");
                }

                var responseDto = MapToResponseDto(createdOrder);

                return CreatedAtAction(nameof(GetOrderById), new { id = createdOrder.OrderID }, responseDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET: api/orders/my-orders 
        [HttpGet("my-orders")]
        public async Task<IActionResult> GetMyOrders()
        {
            var userId = GetCurrentUserId();
            var orders = await _orderService.GetOrdersByUserIdAsync(userId);

            var orderDtos = orders.Select(MapToResponseDto).ToList();

            return Ok(orderDtos);
        }

        // PUT: api/orders/{id}/cancel (Hủy đơn hàng)
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelOrder(int id)
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
                return BadRequest("Không thể hủy đơn hàng này (Có thể đã giao hoặc đã hủy rồi).");

            return Ok(new { message = "Hủy đơn hàng thành công." });
        }

        // 2. API DÙNG CHUNG (SHARED) HOẶC ADMIN
    
        // GET: api/orders/{id} (Chi tiết đơn hàng)
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

        // 3. API QUẢN TRỊ (ADMIN ONLY)
      
        // GET: api/orders (Xem tất cả)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            var orderDtos = orders.Select(MapToResponseDto).ToList();
            return Ok(orderDtos);
        }

        // PUT: api/orders/{id}/status (Cập nhật trạng thái)
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] OrderUpdateStatusDto dto)
        {
            var result = await _orderService.UpdateOrderStatusAsync(id, dto.OrderStatus);

            if (!result)
                return BadRequest("Cập nhật thất bại.");

            return Ok(new { message = "Cập nhật trạng thái thành công." });
        }

        // 4. CÁC HÀM PHỤ TRỢ (PRIVATE HELPER)
      
        // Hàm lấy UserID từ Token
        private int GetCurrentUserId()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                var userClaim = identity.FindFirst(ClaimTypes.NameIdentifier);
                if (userClaim != null && int.TryParse(userClaim.Value, out int userId))
                {
                    return userId;
                }
            }
            return 0;
        }

        // Hàm kiểm tra quyền Admin
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