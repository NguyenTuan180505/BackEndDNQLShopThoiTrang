using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopThoiTrang.API.Services; // IUserService ở đây

namespace ShopThoiTrang.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        // CHỈ CẦN ĐỔI DÒNG NÀY: UserService → IUserService
        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetMe()
        {
            var userIdClaim = User.FindFirst("userId")?.Value;
            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            var user = await _userService.GetById(userId);
            return user == null ? NotFound() : Ok(user);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var list = await _userService.GetAll();
            return Ok(list);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPut("{id}/role")]
        public async Task<IActionResult> UpdateRole(int id, int roleId)
        {
            await _userService.UpdateRole(id, roleId);
            return Ok("Cập nhật vai trò thành công");
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, bool active)
        {
            await _userService.UpdateStatus(id, active);
            return Ok("Cập nhật trạng thái thành công");
        }
    }
}