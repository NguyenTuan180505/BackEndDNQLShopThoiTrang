using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopThoiTrang.API.Dtos.Payment;
using ShopThoiTrang.API.Services;
using System.Security.Claims;

namespace ShopThoiTrang.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Mặc định yêu cầu đăng nhập
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // ⭐ Lấy userId từ claim "userId" trong JWT (KHÔNG dùng NameIdentifier)
        private int GetUserId()
        {
            var id = User.FindFirstValue("userId");

            if (string.IsNullOrEmpty(id))
                throw new UnauthorizedAccessException("JWT không chứa claim userId.");

            return int.Parse(id);
        }

        // ⭐ Kiểm tra role (để dùng cho GetByOrder và GetById)
        private bool IsAdmin() => User.IsInRole("Admin");


        // =======================================================
        // 1) Customer tạo thanh toán
        // =======================================================
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> Create([FromBody] CreatePaymentDto dto)
        {
            try
            {
                var userId = GetUserId();
                var result = await _paymentService.CreateAsync(dto, userId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        // =======================================================
        // 2) Customer xem payment theo OrderID
        // =======================================================
        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetByOrder(int orderId)
        {
            try
            {
                var userId = GetUserId();
                var result = await _paymentService.GetByOrderIdAsync(orderId, userId, IsAdmin());

                if (result == null)
                    return NotFound();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        // =======================================================
        // 3) Customer xem chi tiết payment
        // =======================================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var userId = GetUserId();
                var result = await _paymentService.GetByIdAsync(id, userId, IsAdmin());

                if (result == null)
                    return NotFound();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        // =======================================================
        // 4) Admin xem tất cả giao dịch
        // =======================================================
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _paymentService.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
