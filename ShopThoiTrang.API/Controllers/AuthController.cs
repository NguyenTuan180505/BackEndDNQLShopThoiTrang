using Microsoft.AspNetCore.Mvc;
using ShopThoiTrang.API.Dtos.Auth;
using ShopThoiTrang.API.Services;
using ShopThoiTrang.API.Services.Auth;

namespace ShopThoiTrang.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(
            [FromServices] IUserService userService,
            RegisterDto dto)
        {
            var existing = await userService.GetByEmail(dto.Email);
            if (existing != null)
                return BadRequest("Email đã tồn tại.");

            var user = await userService.Register(new Models.User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
            }, dto.Password);

            return Ok(new { message = "Đăng ký thành công", userId = user.UserID });
        }

        // 🔐 BƯỚC 1: LOGIN → GỬI OTP
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            await _authService.SendOtpAfterLoginAsync(dto.Email, dto.Password);
            return Ok("OTP đã được gửi về email");
        }

        // 🔐 BƯỚC 2: VERIFY OTP → TRẢ JWT
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp(VerifyOtpDto dto)
        {
            var token = await _authService.VerifyOtpAsync(dto.Email, dto.Otp);
            return Ok(new { token });
        }
    }
}
