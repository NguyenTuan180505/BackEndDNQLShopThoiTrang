using Microsoft.AspNetCore.Mvc;

namespace ShopThoiTrang.API.Controllers
{
    [ApiController]
    [Route("api/upload")]
    public class UploadController : ControllerBase
    {
        [HttpPost("image")]
        public async Task<IActionResult> UploadImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
                return BadRequest("Ảnh không hợp lệ");

            var uploadPath = @"D:\react.js\FrontEndDNQLShopThoiTrang\public\images";

            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            // gửi về cho FE /images/xxx.jpg
            return Ok(new { imageUrl = $"/images/{fileName}" });
        }
    }
}
