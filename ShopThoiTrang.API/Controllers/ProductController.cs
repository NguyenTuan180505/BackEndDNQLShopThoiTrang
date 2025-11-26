using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopThoiTrang.API.Dtos.Products;
using ShopThoiTrang.API.Services;

namespace ShopThoiTrangAPI.Controllers
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductsController(IProductService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _service.GetByIdAsync(id);
            if (product == null)
                return NotFound(new { message = "Sản phẩm không tồn tại" });

            return Ok(product);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateDto dto)
        {
            var created = await _service.CreateAsync(dto);
            return Ok(created);
        }

        [Authorize(Roles = "ADMIN")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProductUpdateDto dto)
        {
            var ok = await _service.UpdateAsync(id, dto);
            if (!ok)
                return NotFound(new { message = "Không tìm thấy sản phẩm để cập nhật" });

            return Ok(new { message = "Cập nhật sản phẩm thành công" });
        }

        [Authorize(Roles = "ADMIN")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok)
                return NotFound(new { message = "Không tìm thấy sản phẩm để xoá" });

            return Ok(new { message = "Xóa sản phẩm thành công" });
        }
    }
    // update for version 1
}
