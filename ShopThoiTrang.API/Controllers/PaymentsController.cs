using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopThoiTrang.API.Dtos.Payment;
using ShopThoiTrang.API.Services;
using System.Security.Claims;

namespace ShopThoiTrang.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Mặc định cần login
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        private bool IsAdmin()
        {
            return User.IsInRole("Admin");
        }

        // =======================================================
        // 1) Tạo thanh toán (Chỉ Customer)
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
            catch (Exception)
            {
                return BadRequest(new { message = "Không thể tạo giao dịch." });
            }
        }

        // =======================================================
        // 2) Customer xem thanh toán theo OrderId
        // =======================================================
        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetByOrder(int orderId)
        {
            try
            {
                var userId = GetUserId();
                var isAdmin = IsAdmin();

                var result = await _paymentService.GetByOrderIdAsync(orderId, userId, isAdmin);
                if (result == null)
                    return NotFound();

                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Lỗi khi lấy giao dịch." });
            }
        }

        // =======================================================
        // 3) Customer xem chi tiết thanh toán theo Id
        // =======================================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var userId = GetUserId();
                var isAdmin = IsAdmin();

                var result = await _paymentService.GetByIdAsync(id, userId, isAdmin);
                if (result == null)
                    return NotFound();

                return Ok(result);
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Không thể lấy chi tiết giao dịch." });
            }
        }

        // =======================================================
        // 4) Admin xem tất cả thanh toán
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
            catch
            {
                return BadRequest(new { message = "Không thể lấy danh sách giao dịch." });
            }
        }
    }
}
