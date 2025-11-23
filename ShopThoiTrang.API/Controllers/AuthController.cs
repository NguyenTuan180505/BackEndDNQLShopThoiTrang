using Microsoft.AspNetCore.Mvc;
using ShopThoiTrang.API.Dtos.Auth;
using ShopThoiTrang.API.Models;
using ShopThoiTrang.API.Services;
using ShopThoiTrang.API.Services.Impl; // Vì bạn đang để JwtService ở đây

namespace ShopThoiTrang.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly JwtService _jwt; // Giữ nguyên như bạn đang có

        // Inject đúng 2 thằng bạn đã đăng ký trong Program.cs
        public AuthController(IUserService userService, JwtService jwt)
        {
            _userService = userService;
            _jwt = jwt;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var existing = await _userService.GetByEmail(dto.Email);
            if (existing != null)
                return BadRequest("Email đã tồn tại.");

            var user = await _userService.Register(new User
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
            }, dto.Password);

            return Ok(new { message = "Đăng ký thành công", userId = user.UserID });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userService.GetByEmail(dto.Email);
            if (user == null || !_userService.CheckPassword(user.PasswordHash!, dto.Password))
                return Unauthorized("Sai email hoặc mật khẩu.");

            var token = _jwt.GenerateToken(user);
            return Ok(new { token });
        }
    }
}